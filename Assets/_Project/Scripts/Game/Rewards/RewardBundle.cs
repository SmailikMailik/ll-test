using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Items;

namespace LL.Game.Rewards
{
    internal sealed class RewardBundle
    {
        internal RewardBundleId Id { get; }
        internal RewardGrantMode GrantMode { get; }
        internal IReadOnlyList<IReward> Rewards { get; }

        internal RewardBundle(
            RewardBundleId id,
            RewardGrantMode grantMode,
            IEnumerable<IReward> rewards)
        {
            if (string.IsNullOrWhiteSpace(id.Value))
                throw new ArgumentException("Reward bundle ID must be non-empty.", nameof(id));

            if (Enum.IsDefined(typeof(RewardGrantMode), grantMode) is false)
                throw new ArgumentOutOfRangeException(
                    nameof(grantMode),
                    grantMode,
                    "Reward grant mode is not supported.");

            Id = id;
            GrantMode = grantMode;
            Rewards = CreateRewards(rewards);
        }

        private static IReadOnlyList<IReward> CreateRewards(IEnumerable<IReward> rewards)
        {
            var copy = rewards?.ToArray() ?? Array.Empty<IReward>();

            if (copy.Length == 0)
                throw new ArgumentException(
                    "Reward bundle must contain at least one reward.",
                    nameof(rewards));

            var itemIds = new HashSet<ItemId>();

            for (var index = 0; index < copy.Length; index++)
            {
                if (copy[index] is not ItemReward item)
                    throw new ArgumentException(
                        $"Reward at index {index} must be an {nameof(ItemReward)}.",
                        nameof(rewards));

                if (itemIds.Add(item.ItemId) is false)
                    throw new ArgumentException(
                        $"Duplicate item reward ID '{item.ItemId}' at index {index}.",
                        nameof(rewards));
            }

            return Array.AsReadOnly(copy);
        }
    }
}