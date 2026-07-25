using LL.Game.Currencies;

namespace LL.Purchases
{
    internal interface IPurchase
    {
        int PurchaseId { get; }
        CurrencyId CurrencyId { get; }
        int Price { get; }
    }

    internal sealed class Purchase : IPurchase
    {
        public int PurchaseId { get; }
        public CurrencyId CurrencyId { get; }
        public int Price { get; }

        internal Purchase(int purchaseId, CurrencyId currencyId, int price)
        {
            PurchaseId = purchaseId;
            CurrencyId = currencyId;
            Price = price;
        }
    }
}