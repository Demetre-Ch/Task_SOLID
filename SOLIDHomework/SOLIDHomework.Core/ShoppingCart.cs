using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOLIDHomework.Core
{
    //Taxes Parent
    public interface ITaxCalculator
    {
        decimal CalculateTax(decimal total);
    }

    //Maybe Make ICalculator, With 2 Methods CalculateTax and CalculateDiscount if it can be considered as one Task


    //Discounts Parent
    public interface IDiscountCalculator
    {
        decimal CalculateDiscount(OrderItem orderItem);
    }

    public class GlobalTaxCalculator:ITaxCalculator
    {
        public decimal CalculateTax(decimal total)
        {
            return total * 1.1M;
        }
    }
    
    public class USTaxCalculator:ITaxCalculator
    {
        public decimal CalculateTax(decimal total)
        {
            return total * 1.2M;
        }
    }

    public class UnitDiscountCalculator:IDiscountCalculator
    {
        public decimal CalculateDiscount(OrderItem orderItem)
        {
            decimal unitDiscount = 0;
            if (orderItem.SeassonEndDate <= DateTime.Now)
            {
                unitDiscount = 20;
            }
            return orderItem.Amount * orderItem.Price * (1 - unitDiscount / 100m);
        }
    }

    public class SpecialDiscountCalculator:IDiscountCalculator
    {
        public decimal CalculateDiscount(OrderItem orderItem)
        {
            decimal total = orderItem.Amount * orderItem.Price;
            int setsOfFour = orderItem.Amount / 4;
            return total - setsOfFour * orderItem.Price;
        }

    }

    //there are OCP and SOC violation
    //
    public class ShoppingCart
    {

        private readonly string country;
        private readonly List<OrderItem> orderItems;
        private readonly USTaxCalculator usTaxCalculator;
        private readonly GlobalTaxCalculator globalTaxCalculator;
        private readonly UnitDiscountCalculator unitDiscountCalculator;
        private readonly SpecialDiscountCalculator specialDiscountCalculator;

        public ShoppingCart(string country, USTaxCalculator uSTaxCalculator, GlobalTaxCalculator globalTaxCalculator, UnitDiscountCalculator unitDiscountCalculator, SpecialDiscountCalculator specialDiscountCalculator)
        {
            this.country = country;
            this.usTaxCalculator = uSTaxCalculator;
            this.globalTaxCalculator = globalTaxCalculator;
            this.unitDiscountCalculator = unitDiscountCalculator;
            this.specialDiscountCalculator = specialDiscountCalculator;
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
                   total = unitDiscountCalculator.CalculateDiscount(orderItem);
                }
                    //when buy 4 prodcuts - get one for free!
                else if (orderItem.Type == "Special")
                {
                    total = specialDiscountCalculator.CalculateDiscount(orderItem);
                }

            }
            if(country == "US")
            {
                total = usTaxCalculator.CalculateTax(total)
            }
            else
            {
                total = globalTaxCalculator.CalculateTax(total);
            }

            return total;
        }

       
    }
}
