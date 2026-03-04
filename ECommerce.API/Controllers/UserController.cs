using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.UserDTO;
using ECommerce.Application.IService;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("GetAll")]
    public async Task<GeneralResponse<List<UserDto>>> GetAll()
    {
        return await _userService.GetAll();
    }

    [HttpGet("GetById")]
    public async Task<GeneralResponse<UserDto>> GetById(Guid Id)
    {
        return await _userService.GetByIdAsync(Id);
    }

    [HttpPost("Add")]
    public async Task<GeneralResponse<Guid>> Add(UserInput input)
    {
        // تجربة بدون Login
        Guid dummyUserId = Guid.NewGuid();
        return await _userService.Add(input, dummyUserId);
    }

    [HttpPost("Update")]
    public async Task<GeneralResponse<Guid>> Update(UserUpdateInput input)
    {
        Guid dummyUserId = Guid.NewGuid();
        return await _userService.Update(input, dummyUserId);
    }

    [HttpPost("SoftDelete")]
    public async Task<GeneralResponse<Guid>> SoftDelete(Guid Id)
    {
        return await _userService.SoftDelete(Id);
    }
}