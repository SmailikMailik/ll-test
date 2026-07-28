using System;
using LL.Game.Items;
using LL.User.Core.Amounts;
using VContainer;

namespace LL.Purchasing
{
    internal sealed class PurchaseService : IPurchaseService
    {
        private readonly IUserAmounts<ItemId> _items;
        private readonly IPurchaseConfirmation _confirmation;

        [Inject]
        internal PurchaseService(
            IUserAmounts<ItemId> items,
            IPurchaseConfirmation confirmation)
        {
            _items = items ?? throw new ArgumentNullException(nameof(items));
            _confirmation = confirmation ?? throw new ArgumentNullException(nameof(confirmation));
        }

        public void Purchase(IPurchase purchase, Action onSucceeded, Action onFailed)
        {
            if (purchase == null)
            {
                onFailed?.Invoke();
                return;
            }

            _confirmation.Confirm(
                purchase,
                () => CompletePurchase(purchase, onSucceeded, onFailed),
                onFailed);
        }

        private void CompletePurchase(IPurchase purchase, Action onSucceeded, Action onFailed)
        {
            if (_items.TrySpend(purchase.ItemId, purchase.Price))
                onSucceeded?.Invoke();
            else
                onFailed?.Invoke();
        }
    }
}