using System;
using LL.Game.Purchases;
using LL.Game.Ranks;
using LL.Purchasing;
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

        private IUserProgress _userProgress;
        private IRankProgression _rankProgression;
        private IPurchaseService _purchaseService;

        [Inject]
        private void Construct(
            IUserProgress userProgress,
            IRankProgression rankProgression,
            IPurchaseService purchaseService)
        {
            _userProgress = userProgress ?? throw new ArgumentNullException(nameof(userProgress));
            _rankProgression = rankProgression ?? throw new ArgumentNullException(nameof(rankProgression));
            _purchaseService = purchaseService ?? throw new ArgumentNullException(nameof(purchaseService));
        }

        private void Start()
        {
            _softButton.Clicked
                .Subscribe(_ => PurchasePromotion(PurchaseIds.RankPromotion))
                .AddTo(this);
            _hardButton.Clicked
                .Subscribe(_ => PurchasePromotion(PurchaseIds.InstantRankPromotion))
                .AddTo(this);
        }

        protected override void OnShow()
        {
            var rank = _userProgress.Rank;
            var progress = _rankProgression.GetProgress(rank, _userProgress.Experience);
            var nextRank = progress.HasNextRank ? rank + 1 : rank;

            _currentRankLabel.text = TextFormatter.Number(rank);
            _nextRankLabel.text = TextFormatter.Number(nextRank);

            SetPrice(_softPriceLabel, PurchaseIds.RankPromotion);
            SetPrice(_hardPriceLabel, PurchaseIds.InstantRankPromotion);

            RefreshActions();
        }

        private void SetPrice(TMP_Text label, PurchaseId id)
        {
            if (_purchaseService.TryGetPurchase(id, out var purchase))
            {
                label.text = PurchaseFormatter.GetPriceText(purchase);
                return;
            }

            label.text = string.Empty;
        }

        private void PurchasePromotion(PurchaseId id)
        {
            if (_userProgress.CanPromoteRank is false)
                return;

            _purchaseService.Purchase(
                id,
                CompletePromotion,
                RefreshActions);
        }

        private void CompletePromotion()
        {
            if (_userProgress.TryPromoteRank())
                TryClose();
        }

        private void RefreshActions()
        {
            var hasHardPurchase = _purchaseService.TryGetPurchase(PurchaseIds.InstantRankPromotion, out _);

            _softButton.SetInteractable(false);
            _hardButton.SetInteractable(hasHardPurchase && _userProgress.CanPromoteRank);
        }
    }

    internal sealed class PromotionWindowParameters : IWindowParameters { }
}