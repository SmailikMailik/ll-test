using LL.Game.Items;

namespace LL.Rewards.Models
{
    internal sealed class ItemReward : IReward
    {
        internal ItemId ItemId { get; }
        public int Amount { get; }

        internal ItemReward(ItemId itemId, int amount)
        {
            ItemId = itemId;
            Amount = amount;
        }
    }
}