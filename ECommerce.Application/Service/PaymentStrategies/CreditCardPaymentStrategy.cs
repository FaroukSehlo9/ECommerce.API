using ECommerce.Application.Common.SharedResources;
using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.PaymentDTO;
using ECommerce.Application.IService;
using Microsoft.Extensions.Localization;
using Stripe; // المكتبة الجديدة اللي بنثبتها حالياً
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ECommerce.Application.Service.PaymentStrategies
{
    public class CreditCardPaymentStrategy : IPaymentService
    {
        private readonly IStringLocalizer<GeneralMessages> _localization;

        public CreditCardPaymentStrategy(IStringLocalizer<GeneralMessages> localization)
        {
            _localization = localization;
        }

        public async Task<GeneralResponse<PaymentResult>> ProcessPayment(PaymentRequest request)
        {
            try
            {
                // الـ Validation الأساسي لحماية السيستم
                if (request.Amount <= 0)
                {
                    throw new ArgumentException(_localization["المبلغ المكتوب غير صحيح، يجب أن يكون أكبر من صفر."].Value);
                }

                // محاكاة الـ 404 اللي لسه سايبينها للتيست بناءً على رغبتك
                if (request.Amount == 404)
                {
                    return new GeneralResponse<PaymentResult>(_localization[
                        "فشلت العملية: رصيد الحساب غير كافي لإتمام الدفع عبر بالبطاقة."].Value,
                        HttpStatusCode.BadRequest
                    );
                }

                // 🚀 إنشاء طلب الدفع (Payment Intent) في سيرفرات Stripe
                var options = new PaymentIntentCreateOptions
                {
                    // ملحوظة: Stripe بيتعامل بأصغر وحدة عملة (Cents/قروش)، عشان كدة بنضرب في 100
                    Amount = (long)(request.Amount * 100),
                    Currency = request.Currency.ToLower(), // سترايب يفضل العملة حروف صغيرة (egp / usd)
                    ReceiptEmail = request.CustomerEmail,
                    PaymentMethodTypes = new List<string> { "card" }, // تحديد نوع الدفع كارت ائتمان
                    Metadata = new Dictionary<string, string>
                    {
                        { "OrderId", request.OrderId.ToString() } // بنربط المعاملة برقم الأوردر عشان تظهر عندك في لوحة تحكم سترايب
                    }
                };

                var service = new PaymentIntentService();
                PaymentIntent intent = await service.CreateAsync(options);

                // 1. تجهيز الـ PaymentResult الداخلي بنجاح
                var result = new PaymentResult
                {
                    IsSuccess = true,
                    TransactionId = intent.Id,
                    Message = "تم إنشاء طلب الدفع بالبطاقة بنجاح",
                    PaymentUrl = intent.ClientSecret
                };

                // 2. 🚀 المناداة الصحيحة: باصي الـ result كأول باراميتر والرسالة كالتاني
                // ده هيشغل الـ Constructor الأول اللي بيخلي الـ Success = true تلقائياً ويظبط الـ Status لـ OK
                return new GeneralResponse<PaymentResult>(result, _localization["تمت العملية بنجاح"].Value);
            }
            catch (StripeException stripeEx)
            {
                // لقط الأخطاء الخاصة بـ Stripe حركياً (مثل كارت منتهي، مرفوض، إلخ)
                return new GeneralResponse<PaymentResult>(
                    _localization["خطأ في بوابة الدفع سترايب: "].Value + stripeEx.StripeError.Message,
                    HttpStatusCode.BadRequest
                );
            }
            catch (Exception ex)
            {
                return new GeneralResponse<PaymentResult>(
                    ex.Message + " خطأ في الدفع بالبطاقة " + ex.InnerException?.Message,
                    HttpStatusCode.BadRequest
                );
            }
        }
    }
}