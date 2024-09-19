using System;
using System.Configuration;

namespace SOLIDHomework.Core.Payment
{
    public class PaymentFactory
    {

        public static PaymentBase GetPaymentService(PaymentServiceType serviceType)
        {

            switch (serviceType)
            {
                case PaymentServiceType.PayPal:
                    return new PayPalPayment(ConfigurationManager.AppSettings["accountName"], 
                        ConfigurationManager.AppSettings["password"]);
                case PaymentServiceType.WorldPay:
                    return new WorldPayPayment(ConfigurationManager.AppSettings["BankID"]);
                default:
                    throw new NotImplementedException("No such service.");
            }
        }
    }
}