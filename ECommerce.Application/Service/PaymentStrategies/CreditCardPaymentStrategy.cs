using ECommerce.Application.Common.SharedResources;
using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.OrderDTO;
using ECommerce.Application.DTOS.PaymentDTO;
using ECommerce.Application.IService;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

                if (request.Amount <= 0)
                {
                    throw new ArgumentException(_localization["المبلغ المكتوب غير صحيح، يجب أن يكون أكبر من صفر."].Value);
                }

               

                // الكود الطبيعي الناجح
                var result = new PaymentResult
                {
                    IsSuccess = true,
                    TransactionId = "STRIPE_TXN_" + Guid.NewGuid().ToString().Substring(0, 8),
                    Message = "تم الدفع بالبطاقة بنجاح"
                };

                // 2. نجاح العملية (بنرجع رسالة النجاح والـ OK Status)
                var response = new GeneralResponse<PaymentResult>(_localization["تمت العملية بنجاح"].Value, System.Net.HttpStatusCode.OK);

                // وبنحط الـ result جوه خاصية الـ Resource يدوي بما إن الـ Constructor مبيقبلهاش
                response.Resource = result;

                return response;
            }
            catch (Exception ex)
            {
                // 3. حالة الـ Catch
                return new GeneralResponse<PaymentResult>(ex.Message + "خطأ في الدفع بالبطاقة" + ex.InnerException?.Message, System.Net.HttpStatusCode.BadRequest);


            }
           
        }
    }
}
