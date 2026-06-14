using ECommerce.API.Extentions;
using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.OrderDTO;
using ECommerce.Application.DTOS.PaymentDTO; // 🆕 ضفنا الـ namespace عشان الـ CreatePaymentAttemptDto
using ECommerce.Application.IService;
using ECommerce.Application.Service;
using ECommerce.Application.Service.Generic;
using MagicBroom.APIServices.ActionFilter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Climate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        // 🆕 1. Endpoint لتسجيل محاولة دفع أولية في الـ Database
        [HttpPost("CreatePaymentAttempt")]
        public async Task<GeneralResponse<Guid>> CreatePaymentAttempt([FromBody] CreatePaymentAttemptDto dto)
        {
            return await _OrderService.CreatePaymentAttempt(dto);
        }

        // 🆕 2. Endpoint لتحديث حالة الدفع للأوردر (نجاح أو فشل) بعد رد بوابة الدفع
        [HttpPost("ConfirmPaymentStatus")]
        public async Task<IActionResult> ConfirmPaymentStatus([FromQuery] string transactionId, [FromQuery] bool isSuccess, [FromQuery] string? errorMsg = null)
        {
            var result = await _OrderService.ConfirmPaymentStatus(transactionId, isSuccess, errorMsg);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
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