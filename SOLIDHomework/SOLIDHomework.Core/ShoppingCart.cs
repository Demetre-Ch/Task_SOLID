using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOLIDHomework.Core
{
    //there are OCP and SOC violation
    //
    public class ShoppingCart
    {
        private readonly string country;
        private readonly List<OrderItem> orderItems;

        public ShoppingCart(string country)
        {
            this.country = country;
            orderItems = new List<OrderItem>();
        }

        public IEnumerable<OrderItem> OrderItems
        {
            get { return orderItems; }
        }
        public void Add(OrderItem orderItem)
        {
            orderItems.Add(orderItem);
        }
        public decimal TotalAmount()
        {
            decimal total = 0;
            
            foreach (var orderItem in OrderItems)
            {
                if (orderItem.Type == "Unit")
                {
                    decimal unitDiscount = 0;
                    //appply 20& discount for old seasons
                    if (orderItem.SeassonEndDate <= DateTime.Now)
                    {
                        unitDiscount = 20;
                    }
                    total = orderItem.Amount * orderItem.Price * (1 - unitDiscount / 100m);
                }
                    //when buy 4 prodcuts - get one for free!
                else if (orderItem.Type == "Special")
                {
                    total += orderItem.Amount * orderItem.Price;
                    int setsOfFour = orderItem.Amount / 4;
                    total -= setsOfFour * orderItem.Price; //discount on groups of 4 items
                }

            }
            if(country == "US")
            {
                total *=1.2M;
            }
            else
            {
                total *= 1.1M;
            }
            return total;
        }

       
    }
}
