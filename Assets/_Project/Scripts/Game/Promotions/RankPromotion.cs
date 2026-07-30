using System;
using LL.Game.Payments;
using LL.Game.Ranks;
using LL.Game.Rewards;

namespace LL.Game.Promotions
{
    internal sealed class RankPromotion
    {
        internal RankId RankId { get; }
        internal RankPromotionQuest Quest { get; }
        internal Payment InstantPayment { get; }
        internal RewardId RewardId { get; }

        internal RankPromotion(
            RankId rankId,
            RankPromotionQuest quest,
            Payment instantPayment,
            RewardId rewardId)
        {
            if (string.IsNullOrWhiteSpace(rankId.Value))
                throw new ArgumentException("Promotion rank ID must be non-empty.", nameof(rankId));

            if (string.IsNullOrWhiteSpace(rewardId.Value))
                throw new ArgumentException("Promotion reward ID must be non-empty.", nameof(rewardId));

            RankId = rankId;
            Quest = quest ?? throw new ArgumentNullException(nameof(quest));
            InstantPayment = instantPayment;
            RewardId = rewardId;
        }
    }
}