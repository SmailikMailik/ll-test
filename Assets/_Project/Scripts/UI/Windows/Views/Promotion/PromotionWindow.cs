using System;
using System.Collections.Generic;
using LL.Game.Items;
using LL.Game.Payments;
using LL.Game.Promotions;
using LL.Game.Ranks;
using LL.Presentation.Payments;
using LL.UI.Controls;
using LL.UI.Typography;
using LL.UI.Windows.Flows;
using LL.User.State.Progress;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace LL.UI.Windows.Views.Promotion
{
    internal sealed class PromotionWindow : Window<PromotionWindowParameters>
    {
        [SerializeField] private TMP_Text _currentRankLabel;
        [SerializeField] private TMP_Text _nextRankLabel;

        [FormerlySerializedAs("_softPriceLabel")]
        [SerializeField] private TMP_Text _orderPriceLabel;

        [FormerlySerializedAs("_softButton")]
        [SerializeField] private InteractiveButton _orderButton;

        [FormerlySerializedAs("_hardPriceLabel")]
        [SerializeField] private TMP_Text _instantPriceLabel;

        [FormerlySerializedAs("_hardButton")]
        [SerializeField] private InteractiveButton _instantButton;

        [SerializeField] private NextRankRewardView _nextRankRewardView;
        [SerializeField] private PromotionOrderView _orderView;

        private IUserProgress _userProgress;
        private IRankProgression _rankProgression;
        private RankPromotionFlow _rankPromotionFlow;
        private UpgradeFlow _upgradeFlow;
        private RankPromotion _promotion;

        [Inject]
        private void Construct(
            IUserProgress userProgress,
            IRankProgression rankProgression,
            RankPromotionFlow rankPromotionFlow,
            UpgradeFlow upgradeFlow)
        {
            _userProgress = userProgress ?? throw new ArgumentNullException(nameof(userProgress));
            _rankProgression = rankProgression ?? throw new ArgumentNullException(nameof(rankProgression));
            _rankPromotionFlow = rankPromotionFlow ?? throw new ArgumentNullException(nameof(rankPromotionFlow));
            _upgradeFlow = upgradeFlow ?? throw new ArgumentNullException(nameof(upgradeFlow));
        }

        private void Start()
        {
            _orderButton.Clicked.Subscribe(_ => OnOrderPromotionClicked()).AddTo(this);
            _instantButton.Clicked.Subscribe(_ => OnInstantPromotionClicked()).AddTo(this);
            _orderView.Completed.Subscribe(_ => OnOrderCompleted()).AddTo(this);
        }

        protected override void OnShow()
        {
            var rank = _userProgress.Rank;
            var progress = _rankProgression.GetProgress(rank, _userProgress.Experience);
            var nextRank = progress.HasNextRank ? rank + 1 : rank;

            _currentRankLabel.text = TextFormatter.Number(rank);
            _nextRankLabel.text = TextFormatter.Number(nextRank);
            _nextRankRewardView.ShowNextRank(rank);

            if (_rankPromotionFlow.TryGetPromotion(out _promotion))
            {
                _orderPriceLabel.text = PaymentFormatter.Format(_promotion.OrderPayment);
                _instantPriceLabel.text = PaymentFormatter.Format(_promotion.InstantPayment);
                _orderView.Refresh(
                    _promotion.Requirement,
                    _promotion.OrderDuration,
                    _userProgress.CanPromoteRank);
            }
            else
            {
                _orderPriceLabel.text = string.Empty;
                _instantPriceLabel.text = string.Empty;
            }

            _orderView.gameObject.SetActive(_promotion != null);
            RefreshActions();
        }

        private void PromoteRank(Payment payment)
        {
            if (_promotion == null || _userProgress.CanPromoteRank is false)
                return;

            _rankPromotionFlow.Promote(
                payment,
                OnPromotionSucceeded,
                OnPromotionFailed);
        }

        private void OnOrderPromotionClicked()
        {
            if (_promotion != null)
                PromoteRank(_promotion.OrderPayment);
        }

        private void OnInstantPromotionClicked()
        {
            if (_promotion != null)
                PromoteRank(_promotion.InstantPayment);
        }

        private void OnPromotionSucceeded(IReadOnlyList<ItemAmount> rewardItems)
        {
            _upgradeFlow.CompletePromotion(_userProgress.Rank, rewardItems);
        }

        private void OnPromotionFailed()
        {
            RefreshActions();
        }

        private void OnOrderCompleted()
        {
            RefreshActions();
        }

        private void RefreshActions()
        {
            var hasPromotion = _promotion != null;
            var canPromoteRank = _userProgress.CanPromoteRank;

            _orderView.SetAvailable(hasPromotion && canPromoteRank);
            _orderButton.SetInteractable(
                hasPromotion &&
                canPromoteRank &&
                _orderView.IsCompleted);
            _instantButton.SetInteractable(hasPromotion && canPromoteRank);
        }
    }

    internal sealed class PromotionWindowParameters : IWindowParameters { }
}