using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOLIDHomework.Core.Model;
using SOLIDHomework.Core.Payment;
using SOLIDHomework.Core.Services;

namespace SOLIDHomework.Core
{
    //Order - check inventory, charge money for credit card and online payments, 
    //tips:
    //think about SRP, DI, OCP
    //maybe for each type of payment type make sense to have own Order-based class?
    public class OrderService
    {
        public void Checkout(string username, ShoppingCart shoppingCart, PaymentDetails paymentDetails, bool notifyCustomer)
        {
            if (paymentDetails.PaymentMethod == PaymentMethod.CreditCard
                || paymentDetails.PaymentMethod == PaymentMethod.OnlineOrder)
            {
                ChargeCard(paymentDetails, shoppingCart);
            }
            ReserveInventory(shoppingCart);
            if (paymentDetails.PaymentMethod == PaymentMethod.OnlineOrder)
            {
                if (notifyCustomer)
                {
                    NotifyCustomer(username);
                }
            }
            MyLogger logger = new MyLogger();
            logger.Write("Success checkout");

        }
        public void NotifyCustomer(string username)
        {
            string customerEmail = new UserService().GetByUsername(username).Email;
            if (!String.IsNullOrEmpty(customerEmail))
            {
                try
                {
                    //construct the email message and send it, implementation ignored
                }
                catch (Exception ex)
                {
                    //log the emailing error, implementation ignored
                }
            }
        }

        public void ReserveInventory(ShoppingCart cart)
        {
            foreach (OrderItem item in cart.OrderItems)
            {
                try
                {
                    InventoryService inventoryService = new InventoryService();
                    inventoryService.Reserve(item.Code, item.Amount);

                }
                catch (InsufficientInventoryException ex)
                {
                    throw new OrderException("Insufficient inventory for item " + item.Code, ex);
                }
                catch (Exception ex)
                {
                    throw new OrderException("Problem reserving inventory", ex);
                }
            }
        }
        
        public void ChargeCard(PaymentDetails paymentDetails, ShoppingCart cart)
        {
            PaymentServiceType paymentServiceType;
            Enum.TryParse(ConfigurationManager.AppSettings["paymentType"], out paymentServiceType);
            try
            {
                PaymentBase payment = PaymentFactory.GetPaymentService(paymentServiceType);
                string serviceResponse = payment.Charge(cart.TotalAmount(), new CreditCart()
                    {
                        CardNumber = paymentDetails.CreditCardNumber,
                        ExpiryDate = paymentDetails.ExpiryDate,
                        NameOnCard = paymentDetails.CardholderName
                    });

                if (!serviceResponse.Contains("200OK") && !serviceResponse.Contains("Success"))
                {
                    throw new Exception(String.Format("Error on charge : {0}", serviceResponse));
                }
            }
            catch (AccountBalanceMismatchException ex)
            {
                throw new OrderException("The card gateway rejected the card based on the address provided.", ex);
            }
            catch (Exception ex)
            {
                throw new OrderException("There was a problem with your card.", ex);
            }

        }
    }

    public class MyLogger
    {
        private readonly string filePath;
        public MyLogger()
        {
            filePath = ConfigurationManager.AppSettings["logPath"];
        }
        public void Write(string text)
        {
            using (Stream file = File.OpenWrite(filePath))
            {
                using (StreamWriter writer = new StreamWriter(file))
                {
                    writer.WriteLine(text);
                }
            }
        }
    }

    public class OrderException : Exception
    {
        public OrderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
    public class AccountBalanceMismatchException : Exception
    {
    }
}
