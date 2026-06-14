using ECommerce.Application.Common.SharedResources;
using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.PaymentDTO;
using ECommerce.Application.IService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Service.PaymentStrategies
{
    public class PayPalPaymentStrategy : IPaymentService
    {
        private readonly PayPalConfigService _payPalConfig;
        private readonly IStringLocalizer<GeneralMessages> _localization;

        public PayPalPaymentStrategy(PayPalConfigService payPalConfig, IStringLocalizer<GeneralMessages> localization)
        {
            _payPalConfig = payPalConfig;
            _localization = localization;
        }

        public async Task<GeneralResponse<PaymentResult>> ProcessPayment(PaymentRequest request)
        {
            try
            {
                var client = _payPalConfig.GetClient(); // جلب الكلاينت الجاهز

                var orderRequest = new OrdersCreateRequest();
                orderRequest.RequestBody(new OrderRequest()
                {
                    CheckoutPaymentIntent = "CAPTURE",
                    PurchaseUnits = new List<PurchaseUnitRequest> {
                    new PurchaseUnitRequest {
                        AmountWithBreakdown = new AmountWithBreakdown {
                            CurrencyCode = "USD",
                            Value = request.Amount.ToString("F2")
                        }
                    }
                }
                });

                var response = await client.Execute(orderRequest);
                var result = response.Result<Order>();

                return new GeneralResponse<PaymentResult>(new PaymentResult
                {
                    IsSuccess = true,
                    TransactionId = result.Id,
                    PaymentUrl = result.Links.Find(l => l.Rel == "approve")?.Href
                }, _localization["تمت العملية بنجاح"].Value);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<PaymentResult>(ex.Message, HttpStatusCode.BadRequest);
            }
        }
        // PayPalConfigService.cs
        public class PayPalConfigService
        {
            private readonly IConfiguration _config;
            public PayPalConfigService(IConfiguration config) => _config = config;

            public PayPalHttpClient GetClient()
            {
                var clientId = _config["PayPal:ClientId"];
                var secret = _config["PayPal:ClientSecret"];
                // استخدم SandboxEnvironment للـ Test
                var environment = new SandboxEnvironment(clientId, secret);
                return new PayPalHttpClient(environment);
            }
        }
    }
}
