using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.UserDTO;
using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.IService
{
    public interface IUserService
    {
        Task<GeneralResponse<List<UserDto>>> GetAll();
        Task<GeneralResponse<List<AdminDto>>> GetAdmin();
        Task<GeneralResponse<UserDto>> GetByIdAsync(Guid Id);
        Task<GeneralResponse<Guid>> Add(UserInput Input, Guid UserId);
        Task<GeneralResponse<Guid>> Update(UserUpdateInput Input, Guid UserId);
        Task<GeneralResponse<List<Guid>>> SoftRangeDelete(List<Guid> Id);
        Task<GeneralResponse<Guid>> SoftDelete(Guid Id);
        //Validation 
        bool CheckUser(User User, out string message);


        User GetUserById(Guid UserID);
    }
}
