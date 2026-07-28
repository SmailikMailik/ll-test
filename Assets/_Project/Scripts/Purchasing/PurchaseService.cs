using System;
using LL.User.Core.Items;
using VContainer;

namespace LL.Purchasing
{
    internal sealed class PurchaseService : IPurchaseService
    {
        private readonly IUserItems _items;
        private readonly IPurchaseConfirmation _confirmation;

        [Inject]
        internal PurchaseService(
            IUserItems items,
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