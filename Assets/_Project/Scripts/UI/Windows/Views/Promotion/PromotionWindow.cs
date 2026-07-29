using System;
using System.Collections.Generic;
using LL.Game.Items;
using LL.Game.Promotions;
using LL.Game.Promotions.Services;
using LL.Game.Ranks;
using LL.Presentation.Payments;
using LL.UI.Controls;
using LL.UI.Typography;
using LL.UI.Windows.Flows;
using LL.User.State.Progress;
using R3;
using TMPro;
using UnityEngine;
using VContainer;

namespace LL.UI.Windows.Views.Promotion
{
    internal sealed class PromotionWindow : Window<PromotionWindowParameters>
    {
        [SerializeField] private TMP_Text _currentRankLabel;
        [SerializeField] private TMP_Text _nextRankLabel;

        [SerializeField] private TMP_Text _softPriceLabel;
        [SerializeField] private InteractiveButton _softButton;

        [SerializeField] private TMP_Text _hardPriceLabel;
        [SerializeField] private InteractiveButton _hardButton;

        [SerializeField] private NextRankRewardView _nextRankRewardView;
        [SerializeField] private PromotionOrderView _orderView;

        private IUserProgress _userProgress;
        private IRankProgression _rankProgression;
        private IRankPromotionService _promotionService;
        private UpgradeFlow _upgradeFlow;
        private RankPromotion _promotion;

        [Inject]
        private void Construct(
            IUserProgress userProgress,
            IRankProgression rankProgression,
            IRankPromotionService promotionService,
            UpgradeFlow upgradeFlow)
        {
            _userProgress = userProgress ?? throw new ArgumentNullException(nameof(userProgress));
            _rankProgression = rankProgression ?? throw new ArgumentNullException(nameof(rankProgression));
            _promotionService = promotionService ?? throw new ArgumentNullException(nameof(promotionService));
            _upgradeFlow = upgradeFlow ?? throw new ArgumentNullException(nameof(upgradeFlow));
        }

        private void Start()
        {
            _softButton.Clicked.Subscribe(_ => PromoteRank(PromotionPaymentType.Soft)).AddTo(this);
            _hardButton.Clicked.Subscribe(_ => PromoteRank(PromotionPaymentType.Hard)).AddTo(this);
            _orderView.Completed.Subscribe(_ => RefreshActions()).AddTo(this);
        }

        protected override void OnShow()
        {
            var rank = _userProgress.Rank;
            var progress = _rankProgression.GetProgress(rank, _userProgress.Experience);
            var nextRank = progress.HasNextRank ? rank + 1 : rank;

            _currentRankLabel.text = TextFormatter.Number(rank);
            _nextRankLabel.text = TextFormatter.Number(nextRank);
            _nextRankRewardView.ShowNextRank(rank);

            if (_promotionService.TryGetPromotion(out _promotion))
            {
                _softPriceLabel.text = PaymentFormatter.Format(_promotion.SoftPayment);
                _hardPriceLabel.text = PaymentFormatter.Format(_promotion.HardPayment);
                _orderView.Refresh(
                    _promotion.Requirement,
                    _promotion.OrderDuration,
                    _userProgress.CanPromoteRank);
            }
            else
            {
                _softPriceLabel.text = string.Empty;
                _hardPriceLabel.text = string.Empty;
            }

            _orderView.gameObject.SetActive(_promotion != null);
            RefreshActions();
        }

        private void PromoteRank(PromotionPaymentType paymentType)
        {
            if (_promotion == null || _userProgress.CanPromoteRank is false)
                return;

            _promotionService.Promote(
                paymentType,
                CompletePromotion,
                RefreshActions);
        }

        private void CompletePromotion(IReadOnlyList<ItemAmount> rewardItems)
        {
            _upgradeFlow.CompletePromotion(_userProgress.Rank, rewardItems);
        }

        private void RefreshActions()
        {
            var hasPromotion = _promotion != null;
            var canPromoteRank = _userProgress.CanPromoteRank;

            _orderView.SetAvailable(hasPromotion && canPromoteRank);
            _softButton.SetInteractable(
                hasPromotion &&
                canPromoteRank &&
                _orderView.IsCompleted);
            _hardButton.SetInteractable(hasPromotion && canPromoteRank);
        }
    }

    internal sealed class PromotionWindowParameters : IWindowParameters { }
}