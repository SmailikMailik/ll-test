using LL.Game.Items;

namespace LL.Game.Purchases
{
    internal interface IPurchase
    {
        PurchaseId Id { get; }
        ItemId ItemId { get; }
        int Price { get; }
    }

    internal sealed class Purchase : IPurchase
    {
        public PurchaseId Id { get; }
        public ItemId ItemId { get; }
        public int Price { get; }

        internal Purchase(PurchaseId id, ItemId itemId, int price)
        {
            Id = id;
            ItemId = itemId;
            Price = price;
        }
    }
}