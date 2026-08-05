using System;
using LL.Game.Identifiers;
using LL.Game.Items;
using LL.Validation;

namespace LL.Game.Payments
{
    internal readonly struct Payment
    {
        internal ItemId ItemId { get; }
        internal int Amount { get; }

        internal Payment(ItemId itemId, int amount)
        {
            IdentifierValidator.EnsureValid(itemId, nameof(itemId));

            if (ValidationChecks.IsNonPositive(amount))
                throw new ArgumentOutOfRangeException(nameof(amount));

            ItemId = itemId;
            Amount = amount;
        }
    }
}