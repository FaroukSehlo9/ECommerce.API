using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOS.PaymentDTO
{
    public class PaymentResult
    {
        public bool IsSuccess { get; set; }
        public string TransactionId { get; set; }
        public string Message { get; set; }
        public string PaymentUrl { get; set; }
    }
}
