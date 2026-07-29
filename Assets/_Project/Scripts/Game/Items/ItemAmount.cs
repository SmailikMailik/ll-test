using System;

namespace LL.Game.Items
{
    internal readonly struct ItemAmount
    {
        internal ItemId Id { get; }
        internal int Amount { get; }

        internal ItemAmount(ItemId id, int amount)
        {
            if (string.IsNullOrWhiteSpace(id.Value))
                throw new ArgumentException("Item amount ID must be non-empty.", nameof(id));

            if (amount < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    amount,
                    "Item amount must not be negative.");

            Id = id;
            Amount = amount;
        }
    }
}