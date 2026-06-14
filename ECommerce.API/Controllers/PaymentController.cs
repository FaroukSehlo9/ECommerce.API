using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.OrderDTO;
using ECommerce.Application.DTOS.PaymentDTO;
using ECommerce.Application.IService; // 🆕 ضفنا الـ namespace عشان الـ IOrderService
using ECommerce.Application.Service.PaymentStrategies;
using ECommerce.Domain.IRepositories;
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


        public PaymentController(PaymentStrategyFactory paymentFactory, IOrderService orderService )
        {
            _paymentFactory = paymentFactory;
            _orderService = orderService;
        }

        [HttpPost("process")]
        public async Task<GeneralResponse<PaymentResult>> Process([FromQuery] PaymentMethod method, [FromBody] PaymentProcessRequest request)
        {
            // 1. هنجيب بيانات الأوردر من الداتابيز باستخدام الـ OrderId (الأمان الكامل)
            var order = await _orderService.GetOrderById(request.OrderId);
            if (order == null) return new GeneralResponse<PaymentResult>("Order not found", System.Net.HttpStatusCode.NotFound);

            // 2. هنجهز الـ Request اللي هيروح لـ Stripe/PayPal بالبيانات اللي جبناها من الداتابيز
            var paymentDetails = new PaymentRequest
            {
                OrderId = request.OrderId,
                Amount = order.Resource.TotalPrice, // السعر من الداتابيز (غير قابل للتلاعب)
                Currency = "USD" // تثبيت العملة هنا
            };

            // 3. كمل الـ Flow بتاعك عادي
            var strategy = _paymentFactory.GetPaymentStrategy(method);
            var paymentResponse = await strategy.ProcessPayment(paymentDetails);

            // 4. الربط وتسجيل المحاولة (زي ما إنت عاملها بالضبط)
            if (paymentResponse.Success && !string.IsNullOrEmpty(paymentResponse.Resource?.TransactionId))
            {
                var attemptDto = new CreatePaymentAttemptDto
                {
                    OrderId = request.OrderId,
                    Amount = order.Resource.TotalPrice, // تأكيد السعر من الداتابيز
                    Currency = "USD", // تثبيت العملة هنا
                    Method = (int)method,
                    TransactionId = paymentResponse.Resource.TransactionId
                };
                await _orderService.CreatePaymentAttempt(attemptDto);
            }

            return paymentResponse;


        }
    }
}