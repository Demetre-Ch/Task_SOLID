namespace SOLIDHomework.Core.Payment
{
    public class WorldPayPayment : PaymentBase
    {
        public WorldPayPayment(string appSetting)
        {
            throw new System.NotImplementedException();
        }

        //required for Auth;
        public string BankID { get; set; }
        public string DomenID { get; set; }

        public override string Charge(decimal amount, CreditCart creditCart)
        {
            WorldPayWebService worldPayWebService = new WorldPayWebService();
            string response = worldPayWebService.Charge(amount, creditCart, BankID, DomenID);
            return response;
        }
    }
}