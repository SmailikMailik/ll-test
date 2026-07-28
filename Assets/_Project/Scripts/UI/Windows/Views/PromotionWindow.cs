using System;
using System.Collections.Generic;
using LL.Game.Promotions;
using LL.Game.Ranks;
using LL.Presentation.Localization;
using LL.Presentation.Promotions;
using LL.Promotions;
using LL.Rewards;
using LL.UI.Controls.Buttons;
using LL.UI.Rewards;
using LL.UI.Typography;
using LL.UI.Windows.Flows;
using LL.User.Core.Progress;
using R3;
using TMPro;
using UnityEngine;
using VContainer;

namespace LL.UI.Windows.Views
{
    internal sealed class PromotionWindow : Window<PromotionWindowParameters>
    {
        [SerializeField] private TMP_Text _currentRankLabel;
        [SerializeField] private TMP_Text _nextRankLabel;
        [SerializeField] private TMP_Text _rewardsTitleLabel;
        [SerializeField] private RewardLayout _rewardLayout;

        [SerializeField] private TMP_Text _softPriceLabel;
        [SerializeField] private InteractiveButton _softButton;

        [SerializeField] private TMP_Text _hardPriceLabel;
        [SerializeField] private InteractiveButton _hardButton;

        [SerializeField] private PromotionOrderView _orderView;

        private const string RankVariable = "rank";

        private IUserProgress _userProgress;
        private IRankProgression _rankProgression;
        private IRankPromotionService _promotionService;
        private UpgradeFlow _upgradeFlow;
        private ILocalizationService _localization;
        private RewardBundleCatalog _rewardBundleCatalog;
        private RankPromotion _promotion;

        [Inject]
        private void Construct(
            IUserProgress userProgress,
            IRankProgression rankProgression,
            IRankPromotionService promotionService,
            UpgradeFlow upgradeFlow,
            ILocalizationService localization,
            RewardBundleCatalog rewardBundleCatalog)
        {
            _userProgress = userProgress ?? throw new ArgumentNullException(nameof(userProgress));
            _rankProgression = rankProgression ?? throw new ArgumentNullException(nameof(rankProgression));
            _promotionService = promotionService ?? throw new ArgumentNullException(nameof(promotionService));
            _upgradeFlow = upgradeFlow ?? throw new ArgumentNullException(nameof(upgradeFlow));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _rewardBundleCatalog = rewardBundleCatalog ?? throw new ArgumentNullException(nameof(rewardBundleCatalog));
        }

        private void Start()
        {
            _softButton.Clicked
                .Subscribe(_ => PurchasePromotion(PromotionPaymentType.Soft))
                .AddTo(this);
            _hardButton.Clicked
                .Subscribe(_ => PurchasePromotion(PromotionPaymentType.Hard))
                .AddTo(this);
            _orderView.Completed
                .Subscribe(_ => RefreshActions())
                .AddTo(this);
        }

        protected override void OnShow()
        {
            var rank = _userProgress.Rank;
            var progress = _rankProgression.GetProgress(rank, _userProgress.Experience);
            var nextRank = progress.HasNextRank ? rank + 1 : rank;

            _currentRankLabel.text = TextFormatter.Number(rank);
            _nextRankLabel.text = TextFormatter.Number(nextRank);
            _rewardsTitleLabel.text = _localization.GetText(
                PromotionLocalizationKeys.RewardsAtRank,
                new Dictionary<string, object>
                {
                    [RankVariable] = nextRank
                });

            if (_promotionService.TryGetPromotion(out _promotion))
            {
                _softPriceLabel.text = TextFormatter.ItemAmount(
                    _promotion.SoftPurchase.ItemId,
                    _promotion.SoftPurchase.Price);
                _hardPriceLabel.text = TextFormatter.ItemAmount(
                    _promotion.HardPurchase.ItemId,
                    _promotion.HardPurchase.Price);
                ShowRewards(_promotion);
                _orderView.Refresh(
                    _promotion.Requirement,
                    _promotion.OrderDuration,
                    _userProgress.CanPromoteRank);
            }
            else
            {
                _softPriceLabel.text = string.Empty;
                _hardPriceLabel.text = string.Empty;
                _rewardLayout.Clear();
            }

            _orderView.gameObject.SetActive(_promotion != null);
            RefreshActions();
        }

        private void PurchasePromotion(PromotionPaymentType paymentType)
        {
            if (_promotion == null || _userProgress.CanPromoteRank is false)
                return;

            _promotionService.Purchase(
                paymentType,
                _orderView.IsCompleted,
                CompletePromotion,
                RefreshActions);
        }

        private void CompletePromotion()
        {
            _orderView.Reset();
            _upgradeFlow.ReplaceCurrent();
        }

        private void ShowRewards(RankPromotion promotion)
        {
            if (promotion.RewardBundleId.IsEmpty ||
                _rewardBundleCatalog.TryGetBundle(promotion.RewardBundleId, out var bundle) is false)
            {
                _rewardLayout.Clear();
                return;
            }

            _rewardLayout.SetRewards(bundle.Rewards);
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