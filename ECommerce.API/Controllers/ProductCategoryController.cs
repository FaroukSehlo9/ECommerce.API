using ECommerce.API.Extentions;
using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.CategoryDTO;
using ECommerce.Application.DTOS.ProductCategoryDTO;
using ECommerce.Application.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoryController : BaseApiController
    {
        private readonly IProductCategoryService _ProductCategoryService;

        public ProductCategoryController(IProductCategoryService ProductCategoryService)
        {
            _ProductCategoryService = ProductCategoryService;
        }
        [HttpGet("GetAll")]
        public async Task<GeneralResponse<List<ProductCategoryDto>>> GetAll()
        {

            return await _ProductCategoryService.GetAll();
        }
        [HttpGet("GetById")]
        public async Task<GeneralResponse<ProductCategoryDto>> GetById(Guid Id)
        {

            return await _ProductCategoryService.GetByIdAsync(Id);
        }
        [HttpGet("GetByProudctIdAsync")]
        public async Task<GeneralResponse<ProductCategoryDto>> GetByProudctIdAsync(Guid ProuductId)
        {

            return await _ProductCategoryService.GetByProudctIdAsync(ProuductId);
        }
        [HttpGet("GetByCategoryIdAsync")]
        public async Task<GeneralResponse<ProductCategoryDto>> GetByCategoryIdAsync(Guid CategoryId)
        {

            return await _ProductCategoryService.GetByCategoryIdAsync(CategoryId);
        }
        [HttpPost("Add")]
        public async Task<GeneralResponse<Guid>> Add(ProductCategoryInput Input)
        {
            Guid ProductCategoryId = Guid.Parse(HttpContext.GetUserId());

            return await _ProductCategoryService.Add(Input, ProductCategoryId);
        }

        [HttpPost("Update")]
        public async Task<GeneralResponse<Guid>> Update(ProductCategoryUpdateInput Input)
        {
            Guid ProductCategoryId = Guid.Parse(HttpContext.GetUserId());

            return await _ProductCategoryService.Update(Input, ProductCategoryId);
        }
        [HttpPost("SoftDelete")]
        public async Task<GeneralResponse<Guid>> SoftDelete(Guid Id)
        {

            return await _ProductCategoryService.SoftDelete(Id);
        }
        [HttpPost("SoftRangeDelete")]
        public async Task<GeneralResponse<List<Guid>>> SoftRangeDelete(List<Guid> Id)
        {

            return await _ProductCategoryService.SoftRangeDelete(Id);
        }
    }
}
