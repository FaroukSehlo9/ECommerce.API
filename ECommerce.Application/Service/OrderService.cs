using AutoMapper;
using ECommerce.Application.Common.SharedResources;
using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.CartDTO;
using ECommerce.Application.DTOS.OrderDTO;
using ECommerce.Application.DTOS.PaymentDTO; // 🆕 ضفنا الـ namespace عشان الـ CreatePaymentAttemptDto
using ECommerce.Application.IService;
using ECommerce.Domain.Entities;
using ECommerce.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ECommerce.Application.Helpers.CommenEnum;

namespace ECommerce.Application.Service
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _Imapper;
        private readonly IStringLocalizer<GeneralMessages> _localization;
        private readonly IUnitOfWork _unit;
        private readonly ICartService _CartService;

        public OrderService(IMapper imapper, IStringLocalizer<GeneralMessages> localization, IUnitOfWork unitOfWork, ICartService CartService)
        {
            _Imapper = imapper;
            _localization = localization;
            _unit = unitOfWork;
            _CartService = CartService;
        }

        public async Task<GeneralResponse<Guid>> Checkout(Guid userId)
        {
            try
            {
                var cart = await _unit.Cart
                     .All()
                     .Include(x => x.Items.Where(i => !i.IsDeleted))
                     .ThenInclude(i => i.Product)
                     .FirstOrDefaultAsync(x => x.UserId == userId);

                if (cart == null || !cart.Items.Any())
                    return new GeneralResponse<Guid>(_localization["Cart is empty"].Value, System.Net.HttpStatusCode.BadRequest);

                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Orderstatus = Convert.ToInt32(OrderStatus.Pending),
                    Items = new List<OrderItem>()
                };

                decimal total = 0;

                foreach (var item in cart.Items)
                {
                    if (item.Product.StockQuantity < item.Quantity)
                        return new GeneralResponse<Guid>(_localization["Product out of stock"].Value, System.Net.HttpStatusCode.BadRequest);

                    var orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Product.Price
                    };

                    total += item.Product.Price * item.Quantity;

                    order.Items.Add(orderItem);

                    // تعديل المخزون
                    item.Product.StockQuantity -= item.Quantity;

                    // عمل update رسمي للـ EF Core
                    await _unit.Product.UpdateAsync(item.Product);
                }

                order.TotalPrice = total;

                await _unit.Order.AddAsync(order);

                //cart.Items.Clear();
                await _CartService.ClearCart(userId);

                await _unit.SaveAsync();

                return new GeneralResponse<Guid>(order.Id, _localization["Order created successfully"].Value);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<Guid>(ex.Message + "-" + ex.InnerException?.Message, System.Net.HttpStatusCode.BadRequest);
            }
        }

        // 🆕 الميثود الأولى: تسجيل محاولة دفع أولية بحالة Pending في الجدول المنفصل
        public async Task<GeneralResponse<Guid>> CreatePaymentAttempt(CreatePaymentAttemptDto dto)
        {
            try
            {
                var payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    OrderId = dto.OrderId,
                    Amount = dto.Amount,
                    Currency = dto.Currency,
                    PaymentMethod = dto.Method,
                    TransactionId = dto.TransactionId,
                    PaymentStatus = 0 // 0 تعني Pending حسب الـ Business Logic لجدول الدفع
                };

                await _unit.Payment.AddAsync(payment);
                await _unit.SaveAsync();

                return new GeneralResponse<Guid>(payment.Id, _localization["Payment attempt registered successfully"].Value);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<Guid>(ex.Message + "-" + ex.InnerException?.Message, System.Net.HttpStatusCode.BadRequest);
            }
        }

        // 🆕 الميثود الثانية: تأكيد أو فشل الدفع وتحديث حالة الـ Payment وحالة الـ Order المقابل له
        public async Task<GeneralResponse<bool>> ConfirmPaymentStatus(string transactionId, bool isSuccess, string? errorMsg = null)
        {
            try
            {
                // جلب سجل الدفع مع الأوردر بتاعه بالـ TransactionId
                var payment = await _unit.Payment.All()
                    .Include(p => p.Order)
                    .FirstOrDefaultAsync(x => x.TransactionId == transactionId);

                if (payment == null)
                    return new GeneralResponse<bool>(_localization["Payment record not found"].Value, System.Net.HttpStatusCode.BadRequest);

                if (isSuccess)
                {
                    payment.PaymentStatus = 1; // 1 تعني Success
                    payment.Order.Orderstatus = Convert.ToInt32(OrderStatus.Paid); // تحويل الأوردر لـ Paid
                }
                else
                {
                    payment.PaymentStatus = 2; // 2 تعني Failed
                    payment.ErrorMessage = errorMsg;
                    // الأوردر بيفضل Pending أو يتحول لحالة فشل دفع حسب رغبتك
                }

                await _unit.Payment.UpdateAsync(payment);
                await _unit.Order.UpdateAsync(payment.Order);
                await _unit.SaveAsync();

                return new GeneralResponse<bool>(true, _localization["Payment status updated successfully"].Value);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>(ex.Message + "-" + ex.InnerException?.Message, System.Net.HttpStatusCode.BadRequest);
            }
        }

        public async Task<GeneralResponse<List<OrderDto>>> GetAll()
        {
            var orders = await _unit.Order
                    .All()
                    .Include(x => x.User)
                    .Include(x => x.Items)
                    .ThenInclude(x => x.Product)
                    .ToListAsync();

            var result = orders.Select(order => new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                UserName = order.User.UserName,
                TotalPrice = order.TotalPrice,
                OrderStatus = order.Orderstatus != null ? Enum.GetName(typeof(OrderStatus), order.Orderstatus) : null,
                Items = order.Items.Select(item => new OrderItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList()
            }).ToList();

            return new GeneralResponse<List<OrderDto>>(result, _localization["Succes"].Value, result.Count());
        }

        public async Task<GeneralResponse<List<OrderDto>>> GetUserOrders(Guid userId)
        {
            try
            {
                var orders = await _unit.Order
                    .All()
                    .Where(x => x.UserId == userId)
                    .Include(x => x.User)
                    .Include(x => x.Items)
                    .ThenInclude(x => x.Product)
                    .ToListAsync();

                var result = orders.Select(order => new OrderDto
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    UserName = order.User.UserName,
                    TotalPrice = order.TotalPrice,
                    OrderStatus = order.Orderstatus != null ? Enum.GetName(typeof(OrderStatus), order.Orderstatus) : null,
                    Items = order.Items.Select(item => new OrderItemDto
                    {
                        ProductId = item.ProductId,
                        ProductName = item.Product.Name,
                        Quantity = item.Quantity,
                        Price = item.Price
                    }).ToList()
                }).ToList();

                return new GeneralResponse<List<OrderDto>>(result, "Success", result.Count);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<List<OrderDto>>(ex.Message + "-" + ex.InnerException?.Message, System.Net.HttpStatusCode.BadRequest);
            }
        }

        public async Task<GeneralResponse<OrderDto>> GetOrderById(Guid orderId)
        {
            try
            {
                var order = await _unit.Order
                    .All()
                    .Include(x => x.User)
                    .Include(x => x.Items)
                    .ThenInclude(x => x.Product)
                    .FirstOrDefaultAsync(x => x.Id == orderId);

                if (order == null)
                    return new GeneralResponse<OrderDto>(_localization["Order not found"].Value, System.Net.HttpStatusCode.BadRequest);

                var result = new OrderDto
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    UserName = order.User.UserName,
                    TotalPrice = order.TotalPrice,
                    OrderStatus = order.Orderstatus != null ? Enum.GetName(typeof(OrderStatus), order.Orderstatus) : null,
                    Items = order.Items.Select(item => new OrderItemDto
                    {
                        ProductId = item.ProductId,
                        ProductName = item.Product.Name,
                        Quantity = item.Quantity,
                        Price = item.Price
                    }).ToList()
                };

                return new GeneralResponse<OrderDto>(result, "Success");
            }
            catch (Exception ex)
            {
                return new GeneralResponse<OrderDto>(ex.Message + "-" + ex.InnerException?.Message, System.Net.HttpStatusCode.BadRequest);
            }
        }

        public async Task<GeneralResponse<Guid>> CancelOrder(Guid orderId, Guid userId)
        {
            try
            {
                var order = await _unit.Order
                    .All()
                    .Include(x => x.Items)
                    .ThenInclude(x => x.Product)
                    .FirstOrDefaultAsync(x => x.Id == orderId && x.UserId == userId);

                if (order == null)
                    return new GeneralResponse<Guid>(_localization["Order not found"].Value, System.Net.HttpStatusCode.BadRequest);

                if (order.Orderstatus != Convert.ToInt32(OrderStatus.Pending))
                    return new GeneralResponse<Guid>(_localization["Order cannot be cancelled"].Value, System.Net.HttpStatusCode.BadRequest);

                foreach (var item in order.Items)
                {
                    item.Product.StockQuantity += item.Quantity;
                }

                order.Orderstatus = Convert.ToInt32(OrderStatus.Cancelled);

                await _unit.Order.UpdateAsync(order);
                await _unit.SaveAsync();

                return new GeneralResponse<Guid>(_localization["Order cancelled successfully"].Value, System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<Guid>(ex.Message + "-" + ex.InnerException?.Message, System.Net.HttpStatusCode.BadRequest);
            }
        }
    }
}