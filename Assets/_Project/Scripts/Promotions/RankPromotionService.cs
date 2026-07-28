using System;
using System.Collections.Generic;
using LL.Game.Promotions;
using LL.Purchasing;
using LL.Rewards;
using LL.User.Core.Progress;
using UnityEngine;
using VContainer;

namespace LL.Promotions
{
    internal sealed class RankPromotionService : IRankPromotionService
    {
        private readonly RankPromotionCatalog _catalog;
        private readonly IUserProgress _userProgress;
        private readonly IPurchaseService _purchaseService;
        private readonly IRewardGrantService _rewardGrantService;

        [Inject]
        internal RankPromotionService(
            RankPromotionCatalog catalog,
            IUserProgress userProgress,
            IPurchaseService purchaseService,
            IRewardGrantService rewardGrantService)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _userProgress = userProgress ?? throw new ArgumentNullException(nameof(userProgress));
            _purchaseService = purchaseService ?? throw new ArgumentNullException(nameof(purchaseService));
            _rewardGrantService = rewardGrantService ?? throw new ArgumentNullException(nameof(rewardGrantService));
        }

        public bool TryGetPromotion(out RankPromotion promotion)
        {
            return _catalog.TryGetPromotion(_userProgress.Rank, out promotion);
        }

        public void Purchase(
            PromotionPaymentType paymentType,
            bool requirementCompleted,
            Action<IReadOnlyList<IReward>> onSucceeded,
            Action onFailed)
        {
            if (TryGetPromotion(out var promotion) is false ||
                _userProgress.CanPromoteRank is false ||
                paymentType == PromotionPaymentType.Soft && requirementCompleted is false)
            {
                onFailed?.Invoke();
                return;
            }

            _purchaseService.Purchase(
                promotion.GetPurchase(paymentType),
                () => CompletePromotion(promotion, onSucceeded, onFailed),
                onFailed);
        }

        private void CompletePromotion(
            RankPromotion promotion,
            Action<IReadOnlyList<IReward>> onSucceeded,
            Action onFailed)
        {
            if (_userProgress.Rank != promotion.Rank || _userProgress.TryPromoteRank() is false)
            {
                onFailed?.Invoke();
                return;
            }

            var rewards = GrantReward(promotion);
            onSucceeded?.Invoke(rewards);
        }

        private IReadOnlyList<IReward> GrantReward(RankPromotion promotion)
        {
            if (promotion.RewardBundleId.IsEmpty)
                return Array.Empty<IReward>();

            if (_rewardGrantService.TryGrant(promotion.RewardBundleId, out var rewards))
                return rewards;

            Debug.LogError($"Failed to grant promotion reward bundle: {promotion.RewardBundleId}");
            return Array.Empty<IReward>();
        }
    }
}