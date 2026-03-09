using ECommerce.API.Extentions;
using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.CategoryDTO;
using ECommerce.Application.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : BaseApiController
    {
        private readonly ICategoryService _CategoryService;

        public CategoryController(ICategoryService CategoryService)
        {
            _CategoryService = CategoryService;
        }
        [HttpGet("GetAll")]
        public async Task<GeneralResponse<List<CategoryDto>>> GetAll()
        {

            return await _CategoryService.GetAll();
        }
        [HttpGet("GetById")]
        public async Task<GeneralResponse<CategoryDto>> GetById(Guid Id)
        {

            return await _CategoryService.GetByIdAsync(Id);
        }
        [HttpPost("Add")]
        public async Task<GeneralResponse<Guid>> Add(CategoryInput Input)
        {
            Guid CategoryId = Guid.Parse(HttpContext.GetUserId());

            return await _CategoryService.Add(Input, CategoryId);
        }

        [HttpPost("Update")]
        public async Task<GeneralResponse<Guid>> Update(CategoryUpdateInput Input)
        {
            Guid CategoryId = Guid.Parse(HttpContext.GetUserId());

            return await _CategoryService.Update(Input, CategoryId);
        }
        [HttpPost("SoftDelete")]
        public async Task<GeneralResponse<Guid>> SoftDelete(Guid Id)
        {

            return await _CategoryService.SoftDelete(Id);
        }
        [HttpPost("SoftRangeDelete")]
        public async Task<GeneralResponse<List<Guid>>> SoftRangeDelete(List<Guid> Id)
        {

            return await _CategoryService.SoftRangeDelete(Id);
        }
    }
}
