using ECommerce.Application.Common.SharedResources;
using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.PaymentDTO;
using ECommerce.Application.IService;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Service.PaymentStrategies
{
    public class PayPalPaymentStrategy : IPaymentService
    {
        private readonly IStringLocalizer<GeneralMessages> _localization;
        public PayPalPaymentStrategy(IStringLocalizer<GeneralMessages> localization)
        {
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

             

                // الكود الطبيعي الناجح
                var result = new PaymentResult
                {
                    IsSuccess = true,
                    TransactionId = "PAYPAL_TXN_" + Guid.NewGuid().ToString().Substring(0, 8),
                    PaymentUrl = "https://sandbox.paypal.com/checkout?id=" + request.OrderId,
                    Message = "تم إنشاء رابط دفع بايبال",
                };

                // 2. نجاح العملية (بنرجع رسالة النجاح والـ OK Status)
                return new GeneralResponse<PaymentResult>(result, _localization["تمت العملية بنجاح"].Value);

            }
            catch (Exception ex)
            {
                // 3. حالة الـ Catch
                return new GeneralResponse<PaymentResult>(ex.Message + "خطأ في بوابة بايبال" + ex.InnerException?.Message, System.Net.HttpStatusCode.BadRequest);

               
            }


        }
    }
}
