using ECommerce.API.Controllers;
using ECommerce.API.Extentions;
using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.UserDTO;
using ECommerce.Application.IService;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseApiController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            this._userService = userService;
        }
        [HttpGet("GetAll")]
        public async Task<GeneralResponse<List<UserDto>>> GetAll()
        {
            
            return await _userService.GetAll();
        }
        [HttpGet("GetAdmins")]
        public async Task<GeneralResponse<List<AdminDto>>> GetAdmins()
        {

            return await _userService.GetAdmin();
        }
        [HttpGet("GetById")]
        public async Task<GeneralResponse<UserDto>> GetById(Guid Id)
        {
           
            return await _userService.GetByIdAsync(Id);
        }
        [HttpPost("Add")]
        public async Task<GeneralResponse<Guid>> Add(UserInput Input)
        {
            Guid userId = Guid.Parse(HttpContext.GetUserId());

            return await _userService.Add(Input, userId);
        }

        [HttpPost("Update")]
        public async Task<GeneralResponse<Guid>> Update(UserUpdateInput Input)
        {
            Guid userId = Guid.Parse(HttpContext.GetUserId());

            return await _userService.Update(Input, userId);
        }
        [HttpPost("SoftDelete")]
        public async Task<GeneralResponse<Guid>> SoftDelete(Guid Id)
        {

            return await _userService.SoftDelete(Id);
        }
        [HttpPost("SoftRangeDelete")]
        public async Task<GeneralResponse<List<Guid>>> SoftRangeDelete(List<Guid> Id)
        {
            
            return await _userService.SoftRangeDelete(Id);
        }
    }
}
