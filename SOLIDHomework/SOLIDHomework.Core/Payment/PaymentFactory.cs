using System;
using System.Configuration;

namespace SOLIDHomework.Core.Payment
{
    public interface IPaymentFactory
    {
        public static PaymentBase GetPaymentService(PaymentServiceType paymentServiceType);
    }
    public class PayPalPaymentFactory : IPaymentFactory
    {
        public static PaymentBase GetPaymentService(PaymentServiceType paymentServiceType)
        {
            if (paymentServiceType = PaymentServiceType.PayPal)
            {
                return new PayPalPayment(ConfigurationManager.AppSettings["accountName"],
                        ConfigurationManager.AppSettings["password"]);
            }
        }
    }
    public class WorldPayPaymentFactory : IPaymentFactory
    {
        public static PaymentBase GetPaymentService(PaymentServiceType paymentServiceType)
        {
            if (paymentServiceType = PaymentServiceType.WorldPay)
            {
                return new WorldPayPayment(ConfigurationManager.AppSettings["BankID"]);
            }
        }
    }

    public class PaymentFactory
    {

        public static PaymentBase GetPaymentService(PaymentServiceType serviceType)
        {
            IPaymentFactory _paymentFactory;

            public PaymentBase(IPaymentFactory paymentFactory)
            {
                _paymentFactory = paymentFactory;
            }
            _paymentFactory.GetPaymentService(serviceType);
        }
    }
}