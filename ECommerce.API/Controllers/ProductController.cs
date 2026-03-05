using ECommerce.API.Extentions;
using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.ProductDTO;
using ECommerce.Application.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : BaseApiController
    {
        private readonly IProductService _ProductService;

        public ProductController(IProductService ProductService)
        {
            _ProductService = ProductService;
        }
        [HttpGet("GetAll")]
        public async Task<GeneralResponse<List<ProductDto>>> GetAll()
        {

            return await _ProductService.GetAll();
        }
        [HttpGet("GetById")]
        public async Task<GeneralResponse<ProductDto>> GetById(Guid Id)
        {

            return await _ProductService.GetByIdAsync(Id);
        }
        [HttpPost("Add")]
        public async Task<GeneralResponse<Guid>> Add(ProductInput Input)
        {
            Guid ProductId = Guid.Parse(HttpContext.GetUserId());

            return await _ProductService.Add(Input, ProductId);
        }

        [HttpPost("Update")]
        public async Task<GeneralResponse<Guid>> Update(ProductUpdateInput Input)
        {
            Guid ProductId = Guid.Parse(HttpContext.GetUserId());

            return await _ProductService.Update(Input, ProductId);
        }
        [HttpPost("SoftDelete")]
        public async Task<GeneralResponse<Guid>> SoftDelete(Guid Id)
        {

            return await _ProductService.SoftDelete(Id);
        }
        [HttpPost("SoftRangeDelete")]
        public async Task<GeneralResponse<List<Guid>>> SoftRangeDelete(List<Guid> Id)
        {

            return await _ProductService.SoftRangeDelete(Id);
        }
    }
}
