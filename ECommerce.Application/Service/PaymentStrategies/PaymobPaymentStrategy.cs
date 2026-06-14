using ECommerce.Application.Common.SharedResources;
using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.PaymentDTO;
using ECommerce.Application.IService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Service.PaymentStrategies
{
    public class PaymobPaymentStrategy : IPaymentService
    {
        private readonly IConfiguration _config;
        private readonly IStringLocalizer<GeneralMessages> _localization;

        public PaymobPaymentStrategy(IConfiguration config, IStringLocalizer<GeneralMessages> localization)
        {
            _config = config;
            _localization = localization;
        }

        public async Task<GeneralResponse<PaymentResult>> ProcessPayment(PaymentRequest request)
        {
            try
            {
                if (request.Amount <= 0)
                {
                    throw new ArgumentException(_localization["المبلغ المكتوب غير صحيح، يجب أن يكون أكبر من صفر."].Value);
                }

                var apiKey = _config["Paymob:ApiKey"];
                var integrationIdStr = _config["Paymob:IntegrationId"];
                var iframeIdStr = _config["Paymob:IframeId"];

                if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(integrationIdStr) || string.IsNullOrEmpty(iframeIdStr))
                {
                    return new GeneralResponse<PaymentResult>("Paymob configuration is missing in appsettings.json", HttpStatusCode.InternalServerError);
                }

                if (!int.TryParse(integrationIdStr, out int integrationId))
                {
                    return new GeneralResponse<PaymentResult>("Paymob IntegrationId must be an integer", HttpStatusCode.InternalServerError);
                }

                using var client = new HttpClient();

                // Step 1: Authentication Request
                var authBody = new { api_key = apiKey };
                var authContent = new StringContent(JsonConvert.SerializeObject(authBody), Encoding.UTF8, "application/json");
                var authResponse = await client.PostAsync("https://accept.paymob.com/api/auth/tokens", authContent);

                if (!authResponse.IsSuccessStatusCode)
                {
                    var errorDetails = await authResponse.Content.ReadAsStringAsync();
                    return new GeneralResponse<PaymentResult>($"Paymob authentication failed: {errorDetails}", HttpStatusCode.BadRequest);
                }

                var authResultStr = await authResponse.Content.ReadAsStringAsync();
                dynamic authData = JsonConvert.DeserializeObject(authResultStr);
                string token = authData.token;

                // Step 2: Order Registration
                var orderBody = new
                {
                    auth_token = token,
                    delivery_needed = "false",
                    amount_cents = (long)(request.Amount * 100),
                    currency = request.Currency ?? "EGP",
                    merchant_order_id = request.OrderId.ToString(),
                    items = new object[] { }
                };
                var orderContent = new StringContent(JsonConvert.SerializeObject(orderBody), Encoding.UTF8, "application/json");
                var orderResponse = await client.PostAsync("https://accept.paymob.com/api/ecommerce/orders", orderContent);

                if (!orderResponse.IsSuccessStatusCode)
                {
                    var errorDetails = await orderResponse.Content.ReadAsStringAsync();
                    return new GeneralResponse<PaymentResult>($"Paymob order registration failed: {errorDetails}", HttpStatusCode.BadRequest);
                }

                var orderResultStr = await orderResponse.Content.ReadAsStringAsync();
                dynamic orderData = JsonConvert.DeserializeObject(orderResultStr);
                string paymobOrderId = orderData.id.ToString();

                // Step 3: Payment Key Generation
                var keyBody = new
                {
                    auth_token = token,
                    amount_cents = (long)(request.Amount * 100),
                    expiration = 3600,
                    order_id = paymobOrderId,
                    billing_data = new
                    {
                        apartment = "NA",
                        email = request.CustomerEmail ?? "no-email@example.com",
                        floor = "NA",
                        first_name = "NA",
                        street = "NA",
                        building = "NA",
                        phone_number = "NA",
                        shipping_method = "NA",
                        postal_code = "NA",
                        city = "NA",
                        country = "NA",
                        last_name = "NA",
                        state = "NA"
                    },
                    currency = request.Currency ?? "EGP",
                    integration_id = integrationId,
                    lock_order_to_token = true
                };
                var keyContent = new StringContent(JsonConvert.SerializeObject(keyBody), Encoding.UTF8, "application/json");
                var keyResponse = await client.PostAsync("https://accept.paymob.com/api/acceptance/payment_keys", keyContent);

                if (!keyResponse.IsSuccessStatusCode)
                {
                    var errorDetails = await keyResponse.Content.ReadAsStringAsync();
                    return new GeneralResponse<PaymentResult>($"Paymob payment key generation failed: {errorDetails}", HttpStatusCode.BadRequest);
                }

                var keyResultStr = await keyResponse.Content.ReadAsStringAsync();
                dynamic keyData = JsonConvert.DeserializeObject(keyResultStr);
                string paymentKey = keyData.token;

                var result = new PaymentResult
                {
                    IsSuccess = true,
                    TransactionId = paymobOrderId, // Standard practice is to record Paymob Order ID for tracking callbacks
                    Message = "Paymob token generated successfully",
                    PaymentUrl = $"https://accept.paymob.com/api/acceptance/iframes/{iframeIdStr}?payment_token={paymentKey}"
                };

                return new GeneralResponse<PaymentResult>(result, _localization["تمت العملية بنجاح"].Value);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<PaymentResult>($"Paymob payment failed: {ex.Message}", HttpStatusCode.BadRequest);
            }
        }
    }
}
