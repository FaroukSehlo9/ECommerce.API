using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.PaymentDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.IService
{
    public interface IPaymentService
    {
        Task<GeneralResponse<PaymentResult>> ProcessPayment(PaymentRequest request);
    }
}
