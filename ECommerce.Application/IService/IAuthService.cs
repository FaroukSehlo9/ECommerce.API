using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.AuthDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.IService
{
    public interface IAuthService
    {
        Task<GeneralResponse<AuthResponse>> Login(AuthRequest request);
        Task<GeneralResponse<RegistrationResponse>> Register(RegistrationRequest request);
        

    }
}
