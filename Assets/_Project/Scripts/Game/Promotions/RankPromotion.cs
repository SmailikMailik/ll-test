using System;
using LL.Game.Payments;
using LL.Game.Rewards;

namespace LL.Game.Promotions
{
    internal enum PromotionPaymentType : byte
    {
        Soft = 0,
        Hard = 1
    }

    internal sealed class RankPromotion
    {
        internal int Rank { get; }
        internal RankPromotionRequirement Requirement { get; }
        internal TimeSpan OrderDuration { get; }
        internal Payment SoftPayment { get; }
        internal Payment HardPayment { get; }
        internal RewardId RewardId { get; }

        internal RankPromotion(
            int rank,
            RankPromotionRequirement requirement,
            TimeSpan orderDuration,
            Payment softPayment,
            Payment hardPayment,
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
            SoftPayment = softPayment;
            HardPayment = hardPayment;
            RewardId = rewardId;
        }

        internal Payment GetPayment(PromotionPaymentType paymentType) => paymentType switch
        {
            PromotionPaymentType.Soft => SoftPayment,
            PromotionPaymentType.Hard => HardPayment,
            _ => throw new ArgumentOutOfRangeException(nameof(paymentType))
        };
    }
}