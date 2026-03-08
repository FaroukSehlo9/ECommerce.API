using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.CartDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.IService
{
    public interface ICartService
    {
        Task<GeneralResponse<List<CartDto>>> GetAll();
        Task<GeneralResponse<CartDto>> GetByIdAsync(Guid Id);
        Task<GeneralResponse<Guid>> Add(CartInput input, Guid UserId);
        Task<GeneralResponse<Guid>> Update(CartUpdateInput input, Guid UserId);
        Task<GeneralResponse<List<Guid>>> SoftRangeDelete(List<Guid> Id);
        Task<GeneralResponse<Guid>> SoftDelete(Guid Id);
        Task<GeneralResponse<Guid>> RemoveItemFromCart(Guid productId, Guid userId);
        Task<GeneralResponse<Guid>> ClearCart(Guid userId);
       Task<GeneralResponse<CartDto>>  GetCartByUserId(Guid userId);
    }
}
