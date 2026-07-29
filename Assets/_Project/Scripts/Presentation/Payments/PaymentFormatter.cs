using LL.Payments;
using LL.Presentation.Items;

namespace LL.Presentation.Payments
{
    internal static class PaymentFormatter
    {
        internal static string Format(Payment payment)
        {
            return ItemAmountFormatter.Format(payment.ItemId, payment.Amount);
        }
    }
}