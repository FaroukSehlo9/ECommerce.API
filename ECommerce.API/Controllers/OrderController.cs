using ECommerce.API.Extentions;
using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.OrderDTO;
using ECommerce.Application.IService;
using ECommerce.Application.Service;
using ECommerce.Application.Service.Generic;
using MagicBroom.APIServices.ActionFilter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : BaseApiController
    {
        private readonly IOrderService _OrderService;

        public OrderController(IOrderService OrderService)
        {
            _OrderService = OrderService;
        }


        [HttpPost("Checkout")]
        public async Task<GeneralResponse<Guid>> Checkout()
        {
            Guid userId = Guid.Parse(HttpContext.GetUserId());

            return await _OrderService.Checkout(userId);
        }

        [HttpGet("GetAll")]
        public async Task<GeneralResponse<List<OrderDto>>> GetAll()
        {

            return await _OrderService.GetAll();
        }

        [HttpGet("GetUserOrders")]
        public async Task<GeneralResponse<List<OrderDto>>> GetUserOrders()
        {
            Guid userId = Guid.Parse(HttpContext.GetUserId());

            return await _OrderService.GetUserOrders(userId);
        }

        [HttpGet("GetOrderById")]
        public async Task<GeneralResponse<OrderDto>> GetOrderById(Guid orderId)
        {
            return await _OrderService.GetOrderById(orderId);
        }

        [HttpPost("CancelOrder")]
        public async Task<GeneralResponse<Guid>> CancelOrder(Guid orderId)
        {
            Guid userId = Guid.Parse(HttpContext.GetUserId());

            return await _OrderService.CancelOrder(orderId, userId);
        }
       



    }
}
