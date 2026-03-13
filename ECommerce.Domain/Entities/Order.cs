using ECommerce.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class Order :Auditable
    {
        public Guid UserId { get; set; }

        public User User { get; set; }
        public decimal TotalPrice { get; set; }

        public int Orderstatus { get; set; }

        public ICollection<OrderItem> Items { get; set; }

    }
}
