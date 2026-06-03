using Microsoft.Extensions.DependencyInjection;
using ECommerce.Application.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ECommerce.Application.Helpers.CommenEnum;

namespace ECommerce.Application.Service.PaymentStrategies
{
    public class PaymentStrategyFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PaymentStrategyFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPaymentService GetPaymentStrategy(PaymentMethod method)
        {
            return method switch
            {
                PaymentMethod.CreditCard => _serviceProvider.GetRequiredService<CreditCardPaymentStrategy>(),
                PaymentMethod.PayPal => _serviceProvider.GetRequiredService<PayPalPaymentStrategy>(),
                _ => throw new ArgumentException("طريقة الدفع غير مدعومة حالياً")
            };
        }
    }
}
