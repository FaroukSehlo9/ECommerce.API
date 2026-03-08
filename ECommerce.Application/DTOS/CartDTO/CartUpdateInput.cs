using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOS.CartDTO
{
    public class CartUpdateInput
    {
        public Guid Id { get; set; }
        public List<CartItemUpdateInput> Items { get; set; }
    }
}
