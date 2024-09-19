namespace SOLIDHomework.Core.Payment
{
    public abstract class PaymentBase
    {

        public abstract string Charge(decimal amount, CreditCart creditCart);
    }
}