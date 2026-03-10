using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.ProductCategoryDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.IService
{
    public interface IProductCategoryService
    {
        Task<GeneralResponse<List<ProductCategoryDto>>> GetAll();
        Task<GeneralResponse<ProductCategoryDto>> GetByIdAsync(Guid Id);
        Task<GeneralResponse<ProductCategoryDto>> GetByProudctIdAsync(Guid ProuductId);
        Task<GeneralResponse<ProductCategoryDto>> GetByCategoryIdAsync(Guid CategoryId);
        Task<GeneralResponse<Guid>> Add(ProductCategoryInput input, Guid UserId);
        Task<GeneralResponse<Guid>> Update(ProductCategoryUpdateInput input, Guid UserId);
        Task<GeneralResponse<List<Guid>>> SoftRangeDelete(List<Guid> Id);
        Task<GeneralResponse<Guid>> SoftDelete(Guid Id);
    }
}
