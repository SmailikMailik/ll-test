using System;
using LL.Game.Items;

namespace LL.Game.Rewards
{
    internal sealed class ItemReward : IReward
    {
        internal ItemId ItemId { get; }
        public int Amount { get; }

        internal ItemReward(ItemId itemId, int amount)
        {
            if (string.IsNullOrWhiteSpace(itemId.Value))
                throw new ArgumentException("Reward item ID must be non-empty.", nameof(itemId));

            if (amount <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    amount,
                    "Reward amount must be greater than zero.");

            ItemId = itemId;
            Amount = amount;
        }
    }
}