using LL.User.Core.Wallet;

namespace LL.Purchases
{
    internal interface IPurchase
    {
        int PurchaseId { get; }
        CurrencyType Currency { get; }
        int Price { get; }
    }

    internal sealed class Purchase : IPurchase
    {
        public int PurchaseId { get; }
        public CurrencyType Currency { get; }
        public int Price { get; }

        internal Purchase(int purchaseId, CurrencyType currency, int price)
        {
            PurchaseId = purchaseId;
            Currency = currency;
            Price = price;
        }
    }
}