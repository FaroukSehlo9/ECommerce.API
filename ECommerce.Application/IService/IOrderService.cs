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

        // 🆕 باصينا الـ DTO الجديد هنا عشان الكود يبقى Clean جداً ومفصل لبيانات جدولك
        Task<GeneralResponse<Guid>> CreatePaymentAttempt(CreatePaymentAttemptDto dto);

        Task<GeneralResponse<bool>> ConfirmPaymentStatus(string transactionId, bool isSuccess, string? errorMsg = null);

        Task<GeneralResponse<List<OrderDto>>> GetUserOrders(Guid userId);
        Task<GeneralResponse<OrderDto>> GetOrderById(Guid orderId);
        Task<GeneralResponse<Guid>> CancelOrder(Guid orderId, Guid userId);
    }
}
