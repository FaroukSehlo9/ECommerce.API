using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.ProductDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.IService
{
    public interface IProductService
    {
        Task<GeneralResponse<List<ProductDto>>> GetAll();
        Task<GeneralResponse<ProductDto>> GetByIdAsync(Guid Id);
        Task<GeneralResponse<Guid>> Add(ProductInput input, Guid UserId);
        Task<GeneralResponse<Guid>> Update(ProductUpdateInput input, Guid UserId);
        Task<GeneralResponse<List<Guid>>> SoftRangeDelete(List<Guid> Id);
        Task<GeneralResponse<Guid>> SoftDelete(Guid Id);
    }
}
