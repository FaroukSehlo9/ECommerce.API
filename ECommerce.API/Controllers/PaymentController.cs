using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.OrderDTO;
using ECommerce.Application.DTOS.PaymentDTO;
using ECommerce.Application.IService; // 🆕 ضفنا الـ namespace عشان الـ IOrderService
using ECommerce.Application.Service.PaymentStrategies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static ECommerce.Application.Helpers.CommenEnum;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : BaseApiController
    {
        private readonly PaymentStrategyFactory _paymentFactory;
        private readonly IOrderService _orderService; // 🆕 عملنا Inject للـ OrderService المسؤول عن الداتابيز

        public PaymentController(PaymentStrategyFactory paymentFactory, IOrderService orderService)
        {
            _paymentFactory = paymentFactory;
            _orderService = orderService;
        }

        [HttpPost("process")]
        public async Task<GeneralResponse<PaymentResult>> Process([FromQuery] PaymentMethod method, [FromBody] PaymentRequest request)
        {
            // 1. طلب الـ Strategy الصح من الـ Factory بناءً على طريقة الدفع (Stripe أو PayPal)
            var strategy = _paymentFactory.GetPaymentStrategy(method);

            // 2. تنفيذ عملية الدفع الخارجية مع Stripe/PayPal والحصول على الـ الـ TransactionId والـ ClientSecret
            var paymentResponse = await strategy.ProcessPayment(request);

            // 3. 🚀 الربط السحري: لو العملية نجحت مع بوابة الدفع الخارجي، بنسجل المحاولة فوراً في جدول الـ Payment
            // بنفحص الـ paymentResponse.Resource للتأكد إن الـ TransactionId رجع فعلاً
            if (paymentResponse.Success && paymentResponse.Resource != null && !string.IsNullOrEmpty(paymentResponse.Resource.TransactionId))
            {
                var attemptDto = new CreatePaymentAttemptDto
                {
                    OrderId = request.OrderId,
                    Amount = request.Amount,
                    Currency = request.Currency ?? "USD",
                    Method = (int)method, // تحويل الـ Enum لـ int عشان الداتابيز
                    TransactionId = paymentResponse.Resource.TransactionId // الـ pi_... بتاع سترايب
                };

                // مناداة السيرفيس لعمل Insert رسمي في جدول الـ Payment بحالة Pending
                await _orderService.CreatePaymentAttempt(attemptDto);
            }

            // 4. إرجاع الـ Response النهائي للأنجولر
            return paymentResponse;
        }
    }
}