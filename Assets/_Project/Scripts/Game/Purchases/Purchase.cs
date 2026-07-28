using LL.Game.Items;

namespace LL.Game.Purchases
{
    internal interface IPurchase
    {
        ItemId ItemId { get; }
        int Price { get; }
    }

    internal sealed class Purchase : IPurchase
    {
        public ItemId ItemId { get; }
        public int Price { get; }

        internal Purchase(ItemId itemId, int price)
        {
            ItemId = itemId;
            Price = price;
        }
    }
}