using System;
using LL.Game.Currencies;
using LL.Game.Ranks;
using LL.Purchases;
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
        [SerializeField] private InteractiveButton _softButton;
        [SerializeField] private TMP_Text _softPriceLabel;
        [SerializeField] private InteractiveButton _hardButton;
        [SerializeField] private TMP_Text _hardPriceLabel;

        private const int SoftPrice = 99_900;
        private const int HardPrice = 999;
        private const int SoftPurchaseId = 0;
        private const int HardPurchaseId = 1;

        private static readonly IPurchase _softPurchase = new Purchase(SoftPurchaseId, CurrencyIds.Soft, SoftPrice);
        private static readonly IPurchase _hardPurchase = new Purchase(HardPurchaseId, CurrencyIds.Hard, HardPrice);

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
                .Subscribe(_ => TryPurchasePromotion(_softPurchase))
                .AddTo(this);
            _hardButton.Clicked
                .Subscribe(_ => TryPurchasePromotion(_hardPurchase))
                .AddTo(this);
        }

        protected override void OnShow()
        {
            var rank = _userProgress.Rank;
            var progress = _rankProgression.GetProgress(rank, _userProgress.TotalExperience);
            var nextRank = progress.HasNextRank ? rank + 1 : rank;

            _currentRankLabel.text = TextFormatter.Number(rank);
            _nextRankLabel.text = TextFormatter.Number(nextRank);

            _softPriceLabel.text = TextFormatter.CurrencyAmount(CurrencyIds.Soft, SoftPrice);
            _hardPriceLabel.text = TextFormatter.CurrencyAmount(CurrencyIds.Hard, HardPrice);

            _softButton.SetInteractable(false);
            _hardButton.SetInteractable(_userProgress.CanPromoteRank);
        }

        private void TryPurchasePromotion(IPurchase purchase)
        {
            if (_userProgress.CanPromoteRank is false || _purchaseService.TryPurchase(purchase) is false)
                return;

            if (_userProgress.TryPromoteRank())
                TryClose();
        }
    }

    internal sealed class PromotionWindowParameters : IWindowParameters { }
}