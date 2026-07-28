using System;
using LL.Game.Currencies;
using LL.Game.Purchases;
using LL.Rewards;

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

            Rank = rank;
            Requirement = requirement ?? throw new ArgumentNullException(nameof(requirement));
            OrderDuration = orderDuration;
            SoftPurchase = new Purchase(PurchaseIds.RankPromotion, CurrencyIds.Soft, softPrice);
            HardPurchase = new Purchase(PurchaseIds.InstantRankPromotion, CurrencyIds.Hard, hardPrice);
            RewardBundleId = rewardBundleId;
        }

        internal IPurchase GetPurchase(PromotionPaymentType paymentType)
        {
            return paymentType switch
            {
                PromotionPaymentType.Soft => SoftPurchase,
                PromotionPaymentType.Hard => HardPurchase,
                _ => throw new ArgumentOutOfRangeException(nameof(paymentType))
            };
        }
    }
}