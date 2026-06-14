using ECommerce.Application.Common.SharedResources;
using ECommerce.Application.Communications;
using ECommerce.Application.IService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Stripe;
using System;
using System.Collections.Generic; // مهم جداً
using System.Threading.Tasks;

namespace ECommerce.Application.Service.PaymentStrategies
{
    public class StripeWebhookService : IWebhookHandler
    {
        private readonly IOrderService _orderService;
        private readonly string _webhookSecret;
        private readonly IStringLocalizer<GeneralMessages> _localization;

        public string ProviderName => "stripe";

        public StripeWebhookService(
            IOrderService orderService,
            IConfiguration configuration,
            IStringLocalizer<GeneralMessages> localization)
        {
            _orderService = orderService;
            _webhookSecret = configuration["Stripe:WebhookSecret"];
            _localization = localization;
        }

        // التعديل هنا: استخدام IDictionary بدلاً من IHeaderDictionary
        public async Task<GeneralResponse<bool>> HandleAsync(string payload, IDictionary<string, string> headers)
        {
            try
            {
                // الوصول للهيدر من الـ Dictionary
                // بنستخدم TryGetValue لضمان الأمان لو الهيدر مش موجود
                headers.TryGetValue("Stripe-Signature", out var signature);

                // التحقق من التوقيع والـ Event
                var stripeEvent = EventUtility.ConstructEvent(payload, signature, _webhookSecret);

                if (stripeEvent.Type == "payment_intent.succeeded")
                {
                    var intent = stripeEvent.Data.Object as PaymentIntent;
                    string transactionId = intent.Id;

                    await _orderService.ConfirmPaymentStatus(transactionId, true, null);

                    return new GeneralResponse<bool>(true, _localization["Payment status updated successfully"].Value);
                }

                return new GeneralResponse<bool>(false, _localization["Event not handled"].Value);
            }
            catch (StripeException e)
            {
                return new GeneralResponse<bool>(false, e.Message);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>(false, ex.Message);
            }
        }
    }
}