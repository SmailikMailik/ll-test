using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Identifiers;
using LL.Game.Items;

namespace LL.Game.Rewards
{
    internal sealed class Reward
    {
        internal RewardId Id { get; }
        internal RewardGrantMode GrantMode { get; }
        internal IReadOnlyList<ItemAmount> Items { get; }

        internal Reward(
            RewardId id,
            RewardGrantMode grantMode,
            IEnumerable<ItemAmount> items)
        {
            if (string.IsNullOrWhiteSpace(id.Value))
                throw new ArgumentException("Reward ID must be non-empty.", nameof(id));

            if (Enum.IsDefined(typeof(RewardGrantMode), grantMode) is false)
                throw new ArgumentOutOfRangeException(
                    nameof(grantMode),
                    grantMode,
                    "Reward grant mode is not supported.");

            Id = id;
            GrantMode = grantMode;
            Items = CreateItems(items);
        }

        private static IReadOnlyList<ItemAmount> CreateItems(IEnumerable<ItemAmount> items)
        {
            var copy = items?.ToArray() ?? Array.Empty<ItemAmount>();

            if (copy.Length == 0)
                throw new ArgumentException(
                    "Reward must contain at least one item.",
                    nameof(items));

            IdentifierCollectionValidator.EnsureValid(
                copy,
                item => item.Id,
                nameof(items));

            if (copy.Any(item => item.Amount <= 0))
                throw new ArgumentException("Reward item amounts must be greater than zero.", nameof(items));

            return Array.AsReadOnly(copy);
        }
    }
}