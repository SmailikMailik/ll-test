using LL.Game.Payments;

namespace LL.Game.Payments.Services
{
    internal interface IPaymentService
    {
        bool CanPay(Payment payment);
        bool TryPay(Payment payment);
        bool TryRefund(Payment payment);
    }
}