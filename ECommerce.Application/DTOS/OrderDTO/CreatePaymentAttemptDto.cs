using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOS.OrderDTO
{
    public class CreatePaymentAttemptDto
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";
        public int Method { get; set; }
        public string TransactionId { get; set; } = string.Empty;
    }
}
