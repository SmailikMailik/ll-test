using System;
using LL.Game.Items;

namespace LL.User.Core.Items
{
    internal sealed class ItemStack
    {
        internal ItemId Id { get; }
        internal int Amount { get; }

        internal ItemStack(ItemId id, int amount)
        {
            Id = id;
            Amount = Math.Max(0, amount);
        }
    }
}