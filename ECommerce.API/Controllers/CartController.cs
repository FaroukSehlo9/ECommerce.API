using ECommerce.API.Extentions;
using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.CartDTO;
using ECommerce.Application.IService;
using ECommerce.Application.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : BaseApiController
    {
        private readonly ICartService _CartService;

        public CartController(ICartService CartService)
        {
            _CartService = CartService;
        }
        [HttpGet("GetAll")]
        public async Task<GeneralResponse<List<CartDto>>> GetAll()
        {

            return await _CartService.GetAll();
        }
        [HttpGet("GetById")]
        public async Task<GeneralResponse<CartDto>> GetById(Guid Id)
        {

            return await _CartService.GetByIdAsync(Id);
        }

        [HttpGet("GetCartByUserId")]
        public async Task<GeneralResponse<CartDto>> GetCartByUserId(Guid Id)
        {

            return await _CartService.GetCartByUserId(Id);
        }
        [HttpPost("Add")]
        public async Task<GeneralResponse<Guid>> Add(CartInput Input)
        {
            Guid CartId = Guid.Parse(HttpContext.GetUserId());

            return await _CartService.Add(Input, CartId);
        }

        [HttpPost("UpdateCartItem")]
        public async Task<GeneralResponse<Guid>> Update(CartItemUpdateInput Input)
        {
            Guid CartId = Guid.Parse(HttpContext.GetUserId());

            return await _CartService.Update(Input, CartId);
        }

        // DELETE: api/cart/item/{CartId}
        [HttpPost("RemoveItem")]
        public async Task<GeneralResponse<Guid>> RemoveItem(Guid ItemId)
        {
            var userId = Guid.Parse(HttpContext.GetUserId());
            return await _CartService.RemoveItemFromCart(ItemId, userId);
        }

        // DELETE: api/cart/clear
        [HttpPost("clear")]
        public async Task<GeneralResponse<Guid>> ClearCart()
        {
            var userId = Guid.Parse(HttpContext.GetUserId());
            return await _CartService.ClearCart(userId);
        }
        [HttpPost("SoftDelete")]
        public async Task<GeneralResponse<Guid>> SoftDelete(Guid Id)
        {

            return await _CartService.SoftDelete(Id);
        }
        [HttpPost("SoftRangeDelete")]
        public async Task<GeneralResponse<List<Guid>>> SoftRangeDelete(List<Guid> Id)
        {

            return await _CartService.SoftRangeDelete(Id);
        }
    }
}
