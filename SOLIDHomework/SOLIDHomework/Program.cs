using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOLIDHomework.Core;
using SOLIDHomework.Core.Model;

namespace SOLIDHomework
{
    public class Program
    {
        //Entry point to program
        //You don't have to change logic there
        //Tip: that is good place for composition root
        static void Main(string[] args)
        {

            //Do Not Pass Interfaces directly inside class and Pass them through initializating an object
            OrderService orderService = new OrderService();
            ShoppingCart shoppingCart = new ShoppingCart("US");
            shoppingCart.Add(new OrderItem()
                {
                    Amount = 1,
                    SeassonEndDate =  DateTime.Now,
                    Code =  "Test",
                    Price =  10,
                    Type = "Unit"
                });
            orderService.Checkout("TestUser",shoppingCart,new PaymentDetails()
                {
                   CardholderName = "haha",
                   CreditCardNumber =  "41111111111111",
                   ExpiryDate =  DateTime.Now.AddDays(10),
                   PaymentMethod = PaymentMethod.Cash
                },true);
        }
    }
}
