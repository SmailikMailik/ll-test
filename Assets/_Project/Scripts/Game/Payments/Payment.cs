using System;
using LL.Game.Items;

namespace LL.Game.Payments
{
    internal readonly struct Payment
    {
        internal ItemId ItemId { get; }
        internal int Amount { get; }

        internal Payment(ItemId itemId, int amount)
        {
            if (string.IsNullOrWhiteSpace(itemId.Value))
                throw new ArgumentException("Payment item ID must be non-empty.", nameof(itemId));

            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            ItemId = itemId;
            Amount = amount;
        }
    }
}