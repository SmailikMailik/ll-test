using System;
using System.Collections.Generic;
using System.Linq;

namespace LL.Rewards
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
            if (id.IsEmpty)
                throw new ArgumentException("Reward bundle ID must be non-empty.", nameof(id));

            var copy = rewards?.ToArray() ?? Array.Empty<IReward>();

            if (copy.Length == 0 || copy.All(IsValidReward) is false)
            {
                throw new ArgumentException(
                    "A reward bundle must contain valid rewards with positive amounts.",
                    nameof(rewards));
            }

            if (copy.Select(GetRewardKey).Distinct().Count() != copy.Length)
                throw new ArgumentException("A reward bundle must not contain duplicate rewards.", nameof(rewards));

            Id = id;
            GrantMode = grantMode;
            Rewards = Array.AsReadOnly(copy);
        }

        private static bool IsValidReward(IReward reward)
        {
            return reward != null &&
                   reward.Amount > 0 &&
                   reward switch
                   {
                       CurrencyReward currency => currency.CurrencyId.IsEmpty is false,
                       ItemReward item => item.ItemId.IsEmpty is false,
                       CardReward card => card.CardId.IsEmpty is false,
                       _ => false
                   };
        }

        private static string GetRewardKey(IReward reward)
        {
            return reward switch
            {
                CurrencyReward currency => $"{nameof(CurrencyReward)}:{currency.CurrencyId}",
                ItemReward item => $"{nameof(ItemReward)}:{item.ItemId}",
                CardReward card => $"{nameof(CardReward)}:{card.CardId}",
                _ => string.Empty
            };
        }
    }
}