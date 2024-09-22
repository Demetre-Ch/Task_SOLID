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
    public interface IPaymentService(
    {
        void ProccessPayment(ShoppingCart shoppingCart, PaymentDetails paymentDetails);
    }
    public interface IPaymentService
    {
        string Charge(decimal amount, CreditCard cardDetails);
    }


    public interface INotificationService()
    {
        void NotifyCustomer(string message);
    }

    public interface ILogger()
    {
        private readonly string filePath;
        public void Write(string text);
    }

    public class CreditCardPayment : IPaymentService
    {
        override ProccessPayment(ShoppingCart shoppingCart, PaymentDetails paymentDetails)
        {
            Console.WriteLine("Orderd By Credit Card ...")
        }
    }

    public class OnlinePayment : IPaymentService 
    {
        override ProccessPayment(ShoppingCart shoppingCart, PaymentDetails paymentDetails)
        {
            Console.WriteLine("Orderd Online ...")
        }
    }

    public class EmailNotificationService : INotificationService
    {
        public override NotifyCustomer(string username)
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
    }

    public class OrderService
    {
        private readonly IPaymentService _paymentService;
        private readonly INotificationService _notificationService;
        private readonly ILogger _logger;
        private readonly IInventoryServices _inventoryServices;

        public OrderService(IPaymentService paymentService, INotificationService notificationService, 
            ILogger logger, IInventoryServices inventoryServices)
        {
            _paymentService = paymentService;
            _notificationService = notificationService; 
            _logger = logger;
            _inventoryServices = inventoryServices;
        }

        public void Checkout(string username, ShoppingCart shoppingCart, PaymentDetails paymentDetails, bool notifyCustomer)
        {
            _paymentService.ProccessPayment();
            ReserveInventory(shoppingCart);
            if(notifyCustomer)
            {
                _notificationService.NotifyCustomer(username);
            }
            _logger.Write("Success checkout");

        }

        public void ReserveInventory(ShoppingCart cart)
        {
            foreach (OrderItem item in cart.OrderItems)
            {
                try
                { 
                    _inventoryServices.Reserve(item.Code, item.Amount);
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
        

        /*I Have Issues Here, 
         * I Wanted to use abstract class or interface instead of concrete PaymentFactory Class,
         * So I added Changed It in PaymentFactory.cs as you can see in comment.
         * but it got really messy and i dont know if it was even good idead at first :D
         * So then I thought that meybe dependency is PaymentBase but I see its abstract, I have feeling that here is DI issue
         * but i dont know where and how
        */
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

    public class MyLogger:ILogger
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
