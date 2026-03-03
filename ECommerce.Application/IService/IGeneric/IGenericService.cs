using ECommerce.Application.Communications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.IService.IGeneric
{
    public interface IGenericService<T, DTO, Input, UpdateInput, Spec> where T : class where DTO : class where Input : class where UpdateInput : class where Spec : class
    {
        Task<GeneralResponse<List<DTO>>> GetAll(List<string> includes);
        Task<GeneralResponse<DTO>> GetByIdAsync(Guid Id);
        Task<GeneralResponse<Guid>> Add(Input input, string? TypeInt);
        Task<GeneralResponse<Guid>> Update(UpdateInput input, string? TypeInt);
        Task<GeneralResponse<List<Guid>>> SoftRangeDelete(List<Guid> Id);
        Task<GeneralResponse<Guid>> SoftDelete(Guid Id);

    }
}
