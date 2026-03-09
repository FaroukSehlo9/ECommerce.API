using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.CategoryDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.IService
{
    public interface ICategoryService
    {
        Task<GeneralResponse<List<CategoryDto>>> GetAll();
        Task<GeneralResponse<CategoryDto>> GetByIdAsync(Guid Id);
        Task<GeneralResponse<Guid>> Add(CategoryInput input, Guid UserId);
        Task<GeneralResponse<Guid>> Update(CategoryUpdateInput input, Guid UserId);
        Task<GeneralResponse<List<Guid>>> SoftRangeDelete(List<Guid> Id);
        Task<GeneralResponse<Guid>> SoftDelete(Guid Id);
    }
}
