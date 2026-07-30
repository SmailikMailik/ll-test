namespace LL.Game.Payments
{
    internal interface IPaymentService
    {
        bool CanPay(Payment payment);
        bool TryPay(Payment payment);
        bool TryRefund(Payment payment);
    }
}