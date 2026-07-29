using System;
using System.Collections.Generic;
using LL.Game.Payments;
using LL.Game.Rewards;
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
        private readonly IUserPromotionOrder _promotionOrder;
        private readonly IPaymentService _paymentService;
        private readonly IRewardGrantService _rewardGrantService;
        private readonly IRankPromotionConfirmation _confirmation;

        private bool _isPromotionPending;

        [Inject]
        internal RankPromotionService(
            RankPromotionCatalog catalog,
            IUserProgress userProgress,
            IUserPromotionOrder promotionOrder,
            IPaymentService paymentService,
            IRewardGrantService rewardGrantService,
            IRankPromotionConfirmation confirmation)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _userProgress = userProgress ?? throw new ArgumentNullException(nameof(userProgress));
            _promotionOrder = promotionOrder ?? throw new ArgumentNullException(nameof(promotionOrder));
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
            _rewardGrantService = rewardGrantService ?? throw new ArgumentNullException(nameof(rewardGrantService));
            _confirmation = confirmation ?? throw new ArgumentNullException(nameof(confirmation));
        }

        public bool TryGetPromotion(out RankPromotion promotion)
        {
            return _catalog.TryGetPromotion(_userProgress.Rank, out promotion);
        }

        public void Promote(
            PromotionPaymentType paymentType,
            Action<IReadOnlyList<IReward>> onSucceeded,
            Action onFailed)
        {
            if (_isPromotionPending ||
                TryGetPromotion(out var promotion) is false ||
                CanPromote(promotion, paymentType) is false)
            {
                onFailed?.Invoke();
                return;
            }

            var payment = promotion.GetPayment(paymentType);
            _isPromotionPending = true;
            _confirmation.Confirm(
                payment,
                () => CompletePromotion(promotion, paymentType, payment, onSucceeded, onFailed),
                () => RejectPromotion(onFailed));
        }

        private void CompletePromotion(
            RankPromotion promotion,
            PromotionPaymentType paymentType,
            Payment payment,
            Action<IReadOnlyList<IReward>> onSucceeded,
            Action onFailed)
        {
            if (_isPromotionPending is false ||
                CanPromote(promotion, paymentType) is false ||
                _rewardGrantService.CanGrant(promotion.RewardBundleId) is false ||
                _paymentService.TryPay(payment) is false)
            {
                RejectPromotion(onFailed);
                return;
            }

            if (_userProgress.TryPromoteRank() is false)
            {
                RefundPayment(payment);
                RejectPromotion(onFailed);
                return;
            }

            var rewards = GrantReward(promotion);
            _promotionOrder.ClearOrder();
            _isPromotionPending = false;
            onSucceeded?.Invoke(rewards);
        }

        private bool CanPromote(RankPromotion promotion, PromotionPaymentType paymentType)
        {
            if (_userProgress.Rank != promotion.Rank || _userProgress.CanPromoteRank is false)
                return false;

            return paymentType switch
            {
                PromotionPaymentType.Soft =>
                    _promotionOrder.IsCompleted &&
                    _promotionOrder.RequirementId.Equals(promotion.Requirement.Id),
                PromotionPaymentType.Hard => true,
                _ => false
            };
        }

        private IReadOnlyList<IReward> GrantReward(RankPromotion promotion)
        {
            if (_rewardGrantService.TryGrant(promotion.RewardBundleId, out var rewards))
                return rewards;

            Debug.LogError($"Failed to grant promotion reward bundle: {promotion.RewardBundleId}");
            return Array.Empty<IReward>();
        }

        private void RefundPayment(Payment payment)
        {
            if (_paymentService.TryRefund(payment) is false)
                Debug.LogError($"Failed to refund promotion payment: {payment.Amount} {payment.ItemId}");
        }

        private void RejectPromotion(Action onFailed)
        {
            if (_isPromotionPending is false)
                return;

            _isPromotionPending = false;
            onFailed?.Invoke();
        }
    }
}