using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.AuthDTO;
using ECommerce.Application.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        //Auth
        private readonly IAuthService _authService;
        public AuthController(IAuthService authenticationService)
        {
            _authService = authenticationService;

        }
        [HttpPost("Login")]
        public async Task<GeneralResponse<AuthResponse>> Login(AuthRequest request)
        {

            return await _authService.Login(request);
        }
        [HttpPost("Register")]
        public async Task<GeneralResponse<RegistrationResponse>> Register(RegistrationRequest request)
        {

            return await _authService.Register(request);
        }
    }
}
