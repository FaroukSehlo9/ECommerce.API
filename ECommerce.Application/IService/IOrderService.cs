using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.OrderDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.IService
{
    public interface IOrderService
    {
        Task<GeneralResponse<List<OrderDto>>> GetAll();

        Task<GeneralResponse<Guid>> Checkout(Guid userId);

        Task<GeneralResponse<List<OrderDto>>> GetUserOrders(Guid userId);

        Task<GeneralResponse<OrderDto>> GetOrderById(Guid orderId);
        Task<GeneralResponse<Guid>> CancelOrder(Guid orderId, Guid userId);

    }
}
