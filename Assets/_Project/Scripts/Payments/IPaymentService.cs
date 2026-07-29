namespace LL.Payments
{
    internal interface IPaymentService
    {
        bool TryPay(Payment payment);
        bool TryRefund(Payment payment);
    }
}