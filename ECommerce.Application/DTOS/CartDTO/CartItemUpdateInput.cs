using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOS.CartDTO
{
    public class CartItemUpdateInput
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
