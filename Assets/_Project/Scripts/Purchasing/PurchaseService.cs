using System;
using LL.Game.Purchases;
using LL.User.Core.Wallet;
using VContainer;

namespace LL.Purchasing
{
    internal sealed class PurchaseService : IPurchaseService
    {
        private readonly PurchaseCatalog _catalog;
        private readonly IUserWallet _wallet;
        private readonly IPurchaseConfirmation _confirmation;

        [Inject]
        internal PurchaseService(
            PurchaseCatalog catalog,
            IUserWallet wallet,
            IPurchaseConfirmation confirmation)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _wallet = wallet ?? throw new ArgumentNullException(nameof(wallet));
            _confirmation = confirmation ?? throw new ArgumentNullException(nameof(confirmation));
        }

        public bool TryGetPurchase(PurchaseId id, out IPurchase purchase) =>
            _catalog.TryGetPurchase(id, out purchase);

        public void Purchase(PurchaseId id, Action onSucceeded, Action onFailed)
        {
            if (_catalog.TryGetPurchase(id, out var purchase) is false)
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
            if (_wallet.TrySpend(purchase.CurrencyId, purchase.Price))
                onSucceeded?.Invoke();
            else
                onFailed?.Invoke();
        }
    }
}