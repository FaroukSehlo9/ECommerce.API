using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.PaymentDTO;
using ECommerce.Application.Service.PaymentStrategies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static ECommerce.Application.Helpers.CommenEnum;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : BaseApiController
    {
        private readonly PaymentStrategyFactory _paymentFactory;

        public PaymentController(PaymentStrategyFactory paymentFactory)
        {
            _paymentFactory = paymentFactory;
        }

        [HttpPost("process")]
        public async Task<GeneralResponse<PaymentResult>> Process([FromQuery] PaymentMethod method, [FromBody] PaymentRequest request)
        {
           
                // 1. طلب الـ Strategy الصح من الـ Factory بناءً على طريقة الدفع
                var strategy = _paymentFactory.GetPaymentStrategy(method);

                // 2. تنفيذ عملية الدفع وإرجاع الـ GeneralResponse مباشرة للفرونت إيند
                return await strategy.ProcessPayment(request);
            
            
        }
    }
}