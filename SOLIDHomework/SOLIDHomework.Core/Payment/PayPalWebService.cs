﻿namespace SOLIDHomework.Core.Payment
{
    public class PayPalWebService
    {
        //web based service
        public string GetTransactionToken(string accountName, string password)
        {
            return "Something";
        }

        public string Charge(decimal amount, string token, CreditCart creditCart)
        {
            return "200OK";
        }
    }
}