using System;
using LL.Game.Payments;
using LL.User.State.Items;
using VContainer;

namespace LL.Game.Payments.Services
{
    internal sealed class PaymentService : IPaymentService
    {
        private readonly IUserItemsCommands _items;

        [Inject]
        internal PaymentService(IUserItemsCommands items)
        {
            _items = items ?? throw new ArgumentNullException(nameof(items));
        }

        public bool CanPay(Payment payment) => _items.CanSpend(payment.ItemId, payment.Amount);

        public bool TryPay(Payment payment) => _items.TrySpend(payment.ItemId, payment.Amount);
        public bool TryRefund(Payment payment) => _items.TryAdd(payment.ItemId, payment.Amount);
    }
}