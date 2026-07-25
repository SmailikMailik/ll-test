using LL.User.Core.Wallet;
using VContainer;

namespace LL.Purchases
{
    internal interface IPurchaseService
    {
        bool TryPurchase(IPurchase purchase);
    }

    internal sealed class PurchaseService : IPurchaseService
    {
        private readonly IUserWallet _wallet;

        [Inject]
        internal PurchaseService(IUserWallet wallet)
        {
            _wallet = wallet;
        }

        public bool TryPurchase(IPurchase purchase)
        {
            return purchase != null && _wallet.TrySpend(purchase.CurrencyId, purchase.Price);
        }
    }
}