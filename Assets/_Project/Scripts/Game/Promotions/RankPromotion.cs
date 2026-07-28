using System;
using LL.Game.Items;
using LL.Purchasing;
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
        internal IPurchase SoftPurchase { get; }
        internal IPurchase HardPurchase { get; }
        internal RewardBundleId RewardBundleId { get; }

        internal RankPromotion(
            int rank,
            RankPromotionRequirement requirement,
            TimeSpan orderDuration,
            int softPrice,
            int hardPrice,
            RewardBundleId rewardBundleId)
        {
            if (rank < 1)
                throw new ArgumentOutOfRangeException(nameof(rank));

            if (orderDuration <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(orderDuration));

            if (softPrice <= 0)
                throw new ArgumentOutOfRangeException(nameof(softPrice));

            if (hardPrice <= 0)
                throw new ArgumentOutOfRangeException(nameof(hardPrice));

            if (rewardBundleId.IsEmpty)
                throw new ArgumentException("Promotion reward bundle ID must be non-empty.", nameof(rewardBundleId));

            Rank = rank;
            Requirement = requirement ?? throw new ArgumentNullException(nameof(requirement));
            OrderDuration = orderDuration;
            SoftPurchase = new Purchase(ItemIds.Soft, softPrice);
            HardPurchase = new Purchase(ItemIds.Hard, hardPrice);
            RewardBundleId = rewardBundleId;
        }

        internal IPurchase GetPurchase(PromotionPaymentType paymentType) => paymentType switch
        {
            PromotionPaymentType.Soft => SoftPurchase,
            PromotionPaymentType.Hard => HardPurchase,
            _ => throw new ArgumentOutOfRangeException(nameof(paymentType))
        };
    }
}