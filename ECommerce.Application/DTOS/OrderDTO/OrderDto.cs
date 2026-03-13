using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOS.OrderDTO
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string UserName { get; set; }


        public decimal TotalPrice { get; set; }

        public string OrderStatus { get; set; }

        public List<OrderItemDto> Items { get; set; }
    }
}
