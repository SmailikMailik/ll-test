using LL.Game.Currencies;

namespace LL.Game.Purchases
{
    internal interface IPurchase
    {
        PurchaseId Id { get; }
        CurrencyId CurrencyId { get; }
        int Price { get; }
    }

    internal sealed class Purchase : IPurchase
    {
        public PurchaseId Id { get; }
        public CurrencyId CurrencyId { get; }
        public int Price { get; }

        internal Purchase(PurchaseId id, CurrencyId currencyId, int price)
        {
            Id = id;
            CurrencyId = currencyId;
            Price = price;
        }
    }
}