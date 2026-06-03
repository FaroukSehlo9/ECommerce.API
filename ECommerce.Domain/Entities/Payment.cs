using ECommerce.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class Payment : Auditable
    {
        public Guid OrderId { get; set; }
        public Order Order { get; set; } // Relation N-1 (كل معاملة مربوطة بأوردر)

        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";

        public string? TransactionId { get; set; } // pi_... من سترايب أو بايبال
        public int PaymentMethod { get; set; }     // 0 للفيزا، 1 لبايبال
        public int PaymentStatus { get; set; }     // (Pending = 0, Success = 1, Failed = 2)

        public string? ErrorMessage { get; set; }  // لو العملية فشلت بنسجل السبب هنا
    }
}
