using System;
using LL.Game.Identifiers;
using LL.Validation;

namespace LL.Game.Items
{
    internal readonly struct ItemAmount
    {
        internal ItemId Id { get; }
        internal int Amount { get; }

        internal ItemAmount(ItemId id, int amount)
        {
            IdentifierValidator.EnsureValid(id, nameof(id));

            if (ValidationChecks.IsNegative(amount))
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    amount,
                    "Item amount must not be negative.");

            Id = id;
            Amount = amount;
        }
    }
}