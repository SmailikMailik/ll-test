using System;
using LL.Game.Payments;
using LL.Game.Ranks;
using LL.Game.Rewards;

namespace LL.Game.RankUp
{
    internal sealed class RankUpDefinition
    {
        internal RankId RankId { get; }
        internal RankUpQuest Quest { get; }
        internal Payment InstantPayment { get; }
        internal RewardId RewardId { get; }

        internal RankUpDefinition(
            RankId rankId,
            RankUpQuest quest,
            Payment instantPayment,
            RewardId rewardId)
        {
            if (string.IsNullOrWhiteSpace(rankId.Value))
                throw new ArgumentException("Rank-up rank ID must be non-empty.", nameof(rankId));

            if (string.IsNullOrWhiteSpace(rewardId.Value))
                throw new ArgumentException("Rank-up reward ID must be non-empty.", nameof(rewardId));

            RankId = rankId;
            Quest = quest ?? throw new ArgumentNullException(nameof(quest));
            InstantPayment = instantPayment;
            RewardId = rewardId;
        }
    }
}