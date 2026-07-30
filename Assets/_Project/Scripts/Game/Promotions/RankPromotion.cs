using System;
using LL.Game.Payments;
using LL.Game.Rewards;

namespace LL.Game.Promotions
{
    internal sealed class RankPromotion
    {
        internal int Rank { get; }
        internal RankPromotionRequirement Requirement { get; }
        internal TimeSpan OrderDuration { get; }
        internal Payment OrderPayment { get; }
        internal Payment InstantPayment { get; }
        internal RewardId RewardId { get; }

        internal RankPromotion(
            int rank,
            RankPromotionRequirement requirement,
            TimeSpan orderDuration,
            Payment orderPayment,
            Payment instantPayment,
            RewardId rewardId)
        {
            if (rank < 1)
                throw new ArgumentOutOfRangeException(nameof(rank));

            if (orderDuration <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(orderDuration));

            if (string.IsNullOrWhiteSpace(rewardId.Value))
                throw new ArgumentException("Promotion reward ID must be non-empty.", nameof(rewardId));

            Rank = rank;
            Requirement = requirement ?? throw new ArgumentNullException(nameof(requirement));
            OrderDuration = orderDuration;
            OrderPayment = orderPayment;
            InstantPayment = instantPayment;
            RewardId = rewardId;
        }
    }
}