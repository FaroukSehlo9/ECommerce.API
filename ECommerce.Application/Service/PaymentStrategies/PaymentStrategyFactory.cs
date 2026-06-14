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
                PaymentMethod.Paymob => _serviceProvider.GetRequiredService<PaymobPaymentStrategy>(),
                _ => throw new ArgumentException("طريقة الدفع غير مدعومة حالياً")
            };

            //var strategies = _serviceProvider.GetServices<IPaymentService>();
            //return strategies.FirstOrDefault(s => s.Method == method)
            //       ?? throw new ArgumentException("طريقة الدفع غير مدعومة");
        }
    }
}
