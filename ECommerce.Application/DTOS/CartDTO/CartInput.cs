using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOS.CartDTO
{
    public class CartInput
    {
        public List<CartItemInput> Items { get; set; }

    }
}
