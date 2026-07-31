using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Identifiers;
using LL.Game.Items;
using LL.Validation;

namespace LL.Game.Rewards
{
    internal sealed class Reward
    {
        internal RewardId Id { get; }
        internal IReadOnlyList<ItemAmount> Items { get; }

        internal Reward(
            RewardId id,
            IEnumerable<ItemAmount> items)
        {
            IdentifierValidator.EnsureValid(id, nameof(id));

            Id = id;
            Items = CreateItems(items);
        }

        private static IReadOnlyList<ItemAmount> CreateItems(IEnumerable<ItemAmount> items)
        {
            var copy = items?.ToArray() ?? Array.Empty<ItemAmount>();

            if (ValidationChecks.IsEmpty(copy))
                throw new ArgumentException(
                    "Reward must contain at least one item.",
                    nameof(items));

            IdentifierCollectionValidator.EnsureValid(
                copy,
                item => item.Id,
                nameof(items));

            if (copy.Any(item => ValidationChecks.IsNonPositive(item.Amount)))
                throw new ArgumentException("Reward item amounts must be greater than zero.", nameof(items));

            return Array.AsReadOnly(copy);
        }
    }
}