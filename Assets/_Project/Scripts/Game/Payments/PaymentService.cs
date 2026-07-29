using System;
using LL.User.State.Items;
using VContainer;

namespace LL.Game.Payments
{
    internal sealed class PaymentService : IPaymentService
    {
        private readonly IUserItems _items;

        [Inject]
        internal PaymentService(IUserItems items)
        {
            _items = items ?? throw new ArgumentNullException(nameof(items));
        }

        public bool TryPay(Payment payment) => _items.TrySpend(payment.ItemId, payment.Amount);
        public bool TryRefund(Payment payment) => _items.TryAdd(payment.ItemId, payment.Amount);
    }
}