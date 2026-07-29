using System;
using LL.Payments;
using LL.Rewards.Models;

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
        internal RewardBundleId RewardBundleId { get; }

        internal RankPromotion(
            int rank,
            RankPromotionRequirement requirement,
            TimeSpan orderDuration,
            Payment softPayment,
            Payment hardPayment,
            RewardBundleId rewardBundleId)
        {
            if (rank < 1)
                throw new ArgumentOutOfRangeException(nameof(rank));

            if (orderDuration <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(orderDuration));

            if (rewardBundleId.IsEmpty)
                throw new ArgumentException("Promotion reward bundle ID must be non-empty.", nameof(rewardBundleId));

            Rank = rank;
            Requirement = requirement ?? throw new ArgumentNullException(nameof(requirement));
            OrderDuration = orderDuration;
            SoftPayment = softPayment;
            HardPayment = hardPayment;
            RewardBundleId = rewardBundleId;
        }

        internal Payment GetPayment(PromotionPaymentType paymentType) => paymentType switch
        {
            PromotionPaymentType.Soft => SoftPayment,
            PromotionPaymentType.Hard => HardPayment,
            _ => throw new ArgumentOutOfRangeException(nameof(paymentType))
        };
    }
}