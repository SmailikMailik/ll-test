using System;
using System.Collections.Generic;
using LL.Game.Items;
using LL.Game.Payments;
using LL.Game.Payments.Services;
using LL.Game.Rewards.Services;
using LL.User.State.Progress;
using LL.User.State.Promotions;
using UnityEngine;
using VContainer;

namespace LL.Game.Promotions.Services
{
    internal sealed class RankPromotionService : IRankPromotionService
    {
        private readonly RankPromotionCatalog _catalog;
        private readonly IUserProgress _userProgress;
        private readonly IUserPromotionQuest _promotionQuest;
        private readonly IPaymentService _paymentService;
        private readonly IRewardGrantService _rewardGrantService;

        [Inject]
        internal RankPromotionService(
            RankPromotionCatalog catalog,
            IUserProgress userProgress,
            IUserPromotionQuest promotionQuest,
            IPaymentService paymentService,
            IRewardGrantService rewardGrantService)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _userProgress = userProgress ?? throw new ArgumentNullException(nameof(userProgress));
            _promotionQuest = promotionQuest ?? throw new ArgumentNullException(nameof(promotionQuest));
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
            _rewardGrantService = rewardGrantService ?? throw new ArgumentNullException(nameof(rewardGrantService));
        }

        public bool TryGetPromotion(out RankPromotion promotion)
        {
            return _catalog.TryGetPromotion(_userProgress.RankId, out promotion);
        }

        public bool CanPromote(Payment payment)
        {
            return TryGetPromotion(out var promotion) && CanPromote(promotion, payment);
        }

        public bool TryPromote(
            Payment payment,
            out IReadOnlyList<ItemAmount> rewardItems)
        {
            rewardItems = Array.Empty<ItemAmount>();

            if (TryGetPromotion(out var promotion) is false ||
                CanPromote(promotion, payment) is false)
                return false;

            if (_rewardGrantService.CanGrant(promotion.RewardId) is false ||
                _paymentService.TryPay(payment) is false)
                return false;

            if (_userProgress.TryPromoteRank() is false)
            {
                RefundPayment(payment);
                return false;
            }

            rewardItems = GrantReward(promotion);
            _promotionQuest.ClearQuest();
            return true;
        }

        private bool CanPromote(RankPromotion promotion, Payment payment)
        {
            if (_userProgress.RankId.Equals(promotion.RankId) is false ||
                _userProgress.CanPromoteRank is false)
            {
                return false;
            }

            if (Matches(payment, promotion.Quest.Payment))
            {
                return _promotionQuest.IsCompleted &&
                       _promotionQuest.QuestId.Equals(promotion.Quest.QuestId);
            }

            return Matches(payment, promotion.InstantPayment);
        }

        private static bool Matches(Payment payment, Payment expected)
        {
            return payment.ItemId.Equals(expected.ItemId) && payment.Amount == expected.Amount;
        }

        private IReadOnlyList<ItemAmount> GrantReward(RankPromotion promotion)
        {
            if (_rewardGrantService.TryGrant(promotion.RewardId, out var items))
                return items;

            Debug.LogError($"Failed to grant promotion reward: {promotion.RewardId}");
            return Array.Empty<ItemAmount>();
        }

        private void RefundPayment(Payment payment)
        {
            if (_paymentService.TryRefund(payment) is false)
                Debug.LogError($"Failed to refund promotion payment: {payment.Amount} {payment.ItemId}");
        }
    }
}