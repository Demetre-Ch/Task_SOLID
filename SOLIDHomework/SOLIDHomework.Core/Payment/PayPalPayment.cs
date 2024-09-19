namespace SOLIDHomework.Core.Payment
{
    public class PayPalPayment : PaymentBase
    {
        public PayPalPayment(string appSetting, string s)
        {
            throw new System.NotImplementedException();
        }

        //required for Auth;
        public string AccountName { get; set; }
        public string Password { get; set; }

        public override string Charge(decimal amount, CreditCart creditCart)
        {
            PayPalWebService payPalWebService = new PayPalWebService();
            string token = payPalWebService.GetTransactionToken(AccountName, Password);
            string response = payPalWebService.Charge(amount, token, creditCart);
            return response;
        }
    }
}