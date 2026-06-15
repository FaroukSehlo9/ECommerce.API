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
        private readonly IHttpClientFactory _httpClientFactory; // يُفضل استخدامه بدلاً من new HttpClient()

        public PaymobPaymentStrategy(IConfiguration config, IStringLocalizer<GeneralMessages> localization, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _localization = localization;
            _httpClientFactory = httpClientFactory;
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

                var client = _httpClientFactory.CreateClient();

                // Step 1: Authentication
                var authBody = new { api_key = apiKey };
                var authContent = new StringContent(JsonConvert.SerializeObject(authBody), Encoding.UTF8, "application/json");
                var authResponse = await client.PostAsync("https://accept.paymob.com/api/auth/tokens", authContent);

                if (!authResponse.IsSuccessStatusCode)
                {
                    return new GeneralResponse<PaymentResult>("Paymob authentication failed.", HttpStatusCode.BadRequest);
                }

                dynamic authData = JsonConvert.DeserializeObject(await authResponse.Content.ReadAsStringAsync());
                string token = authData.token;

                // Step 2: Order Registration (Fixed duplicate issue)
                var orderBody = new
                {
                    auth_token = token,
                    delivery_needed = "false",
                    amount_cents = (long)(request.Amount * 100),
                    currency = request.Currency ?? "EGP",
                    // تم إضافة Ticks لجعل المعرف فريداً دائماً أثناء الاختبار
                    merchant_order_id = $"{request.OrderId}_{DateTime.UtcNow.Ticks}",
                    items = new object[] { }
                };

                var orderContent = new StringContent(JsonConvert.SerializeObject(orderBody), Encoding.UTF8, "application/json");
                var orderResponse = await client.PostAsync("https://accept.paymob.com/api/ecommerce/orders", orderContent);

                if (!orderResponse.IsSuccessStatusCode)
                {
                    return new GeneralResponse<PaymentResult>($"Paymob order registration failed: {await orderResponse.Content.ReadAsStringAsync()}", HttpStatusCode.BadRequest);
                }

                dynamic orderData = JsonConvert.DeserializeObject(await orderResponse.Content.ReadAsStringAsync());
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
                    currency =  "EGP",
                    integration_id = integrationId,
                    lock_order_to_token = true
                };

                var keyContent = new StringContent(JsonConvert.SerializeObject(keyBody), Encoding.UTF8, "application/json");
                var keyResponse = await client.PostAsync("https://accept.paymob.com/api/acceptance/payment_keys", keyContent);

                if (!keyResponse.IsSuccessStatusCode)
                {
                    // تعديل هنا: اقرأ محتوى الخطأ الفعلي بدلاً من الرسالة الثابتة
                    var errorDetails = await keyResponse.Content.ReadAsStringAsync();
                    return new GeneralResponse<PaymentResult>($"Paymob payment key generation failed: {errorDetails}", HttpStatusCode.BadRequest);
                }

                dynamic keyData = JsonConvert.DeserializeObject(await keyResponse.Content.ReadAsStringAsync());
                string paymentKey = keyData.token;

                var result = new PaymentResult
                {
                    IsSuccess = true,
                    TransactionId = paymobOrderId,
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