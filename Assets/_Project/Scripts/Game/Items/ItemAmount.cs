using System;

namespace LL.Game.Items
{
    internal readonly struct ItemAmount
    {
        internal ItemId Id { get; }
        internal int Amount { get; }

        internal ItemAmount(ItemId id, int amount)
        {
            Id = id;
            Amount = Math.Max(0, amount);
        }
    }
}