using ECommerce.Application.Communications;
using ECommerce.Application.IService;
using Newtonsoft.Json.Linq;
using System.Collections.Generic; // مهم جداً عشان IDictionary
using System.Threading.Tasks;

namespace ECommerce.Application.Service.PaymentStrategies
{
    public class PayPalWebhookService : IWebhookHandler
    {
        private readonly IOrderService _orderService;
        public string ProviderName => "paypal";

        public PayPalWebhookService(IOrderService orderService) => _orderService = orderService;

        // هنا التعديل: استخدم IDictionary بدلاً من IHeaderDictionary
        public async Task<GeneralResponse<bool>> HandleAsync(string payload, IDictionary<string, string> headers)
        {
            if (string.IsNullOrWhiteSpace(payload))
                return new GeneralResponse<bool>(false, "Payload is empty");
            // ملاحظة: لو كنت محتاج تستخدم الـ headers، هتتعامل معاه كـ Dictionary عادي:
            // var transmissionSig = headers.ContainsKey("PAYPAL-TRANSMISSION-SIG") ? headers["PAYPAL-TRANSMISSION-SIG"] : null;

            var json = JObject.Parse(payload);
            if (json["event_type"]?.ToString() == "PAYMENT.CAPTURE.COMPLETED")
            {
                var transactionId = json["resource"]?["id"]?.ToString();
                await _orderService.ConfirmPaymentStatus(transactionId, true, null);
                return new GeneralResponse<bool>(true, "Success");
            }
            return new GeneralResponse<bool>(false, "Event not handled");
        }
    }
}
