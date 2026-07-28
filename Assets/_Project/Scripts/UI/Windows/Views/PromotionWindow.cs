using System;
using LL.Game.Promotions;
using LL.Game.Ranks;
using LL.Promotions;
using LL.UI.Controls.Buttons;
using LL.UI.Typography;
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

        [SerializeField] private TMP_Text _softPriceLabel;
        [SerializeField] private InteractiveButton _softButton;

        [SerializeField] private TMP_Text _hardPriceLabel;
        [SerializeField] private InteractiveButton _hardButton;

        [SerializeField] private PromotionOrderView _orderView;

        private IUserProgress _userProgress;
        private IRankProgression _rankProgression;
        private IRankPromotionService _promotionService;
        private RankPromotion _promotion;

        [Inject]
        private void Construct(
            IUserProgress userProgress,
            IRankProgression rankProgression,
            IRankPromotionService promotionService)
        {
            _userProgress = userProgress ?? throw new ArgumentNullException(nameof(userProgress));
            _rankProgression = rankProgression ?? throw new ArgumentNullException(nameof(rankProgression));
            _promotionService = promotionService ?? throw new ArgumentNullException(nameof(promotionService));
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

            if (_promotionService.TryGetPromotion(out _promotion))
            {
                _softPriceLabel.text = PurchaseFormatter.GetPriceText(_promotion.SoftPurchase);
                _hardPriceLabel.text = PurchaseFormatter.GetPriceText(_promotion.HardPurchase);
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
            TryClose();
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