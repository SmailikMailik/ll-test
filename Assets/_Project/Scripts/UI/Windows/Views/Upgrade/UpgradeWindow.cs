using System;
using LL.UI.Controls.Buttons;
using LL.User.Core.Cards;
using R3;
using UnityEngine;
using VContainer;

namespace LL.UI.Windows.Views.Upgrade
{
    internal sealed class UpgradeWindow : Window<UpgradeWindowParameters>
    {
        [SerializeField] private UpgradeProgressView _progressView;
        [SerializeField] private UpgradeCardSelector _cardSelector;
        [SerializeField] private UpgradeAmountStepper _amountStepper;
        [SerializeField] private InteractiveButton _useButton;

        private const int AmountDelta = 1;
        private const int MinimumAmount = 0;

        private IUserCards _userCards;
        private bool _isApplying;

        [Inject]
        private void Construct(IUserCards userCards)
        {
            _userCards = userCards ?? throw new ArgumentNullException(nameof(userCards));
            _amountStepper.Initialize(AmountDelta);

            _cardSelector.SelectionChanged
                .Subscribe(_ => OnSelectionChanged())
                .AddTo(this);
            _cardSelector.SelectedAmountChanged
                .Subscribe(_ => OnSelectedAmountChanged())
                .AddTo(this);
            _amountStepper.ValueChanged
                .Subscribe(OnPlannedAmountChanged)
                .AddTo(this);
            _useButton.Clicked
                .Subscribe(_ => ApplyCards())
                .AddTo(this);
        }

        protected override void OnShow()
        {
            _progressView.ResetPreview();
            _cardSelector.ResetSelection();
            _amountStepper.ResetValue();
            RefreshUseButton();
        }

        protected override void OnHide()
        {
            _cardSelector.SetPlannedAmount(MinimumAmount);
            _amountStepper.ResetValue();
            _progressView.ClearPreview();
        }

        private void OnSelectionChanged()
        {
            if (_isApplying)
                return;

            _cardSelector.SetPlannedAmount(MinimumAmount);
            _progressView.ClearPreview();
            _amountStepper.ResetValue(GetMaximumAmount());
            RefreshUseButton();
        }

        private void OnSelectedAmountChanged()
        {
            if (_isApplying)
                return;

            _amountStepper.SetMaximum(GetMaximumAmount());
            RefreshUseButton();
        }

        private void OnPlannedAmountChanged(int amount)
        {
            _cardSelector.SetPlannedAmount(amount);

            if (_cardSelector.HasSelection)
            {
                _progressView.SetPendingItems(
                    amount,
                    _cardSelector.SelectedCard.ExperienceAmount);
            }
            else
            {
                _progressView.ClearPreview();
            }

            RefreshUseButton();
        }

        private void ApplyCards()
        {
            if (CanUse() is false)
                return;

            var cardId = _cardSelector.SelectedCard.Id;
            var amount = _amountStepper.Value;

            _isApplying = true;

            if (_userCards.TrySpend(cardId, amount) is false)
            {
                _isApplying = false;
                SynchronizeControls();
                return;
            }

            if (_progressView.TryApplyPendingExperience() is false)
                _userCards.TryAdd(cardId, amount);

            _isApplying = false;
            SynchronizeControls();
        }

        private void SynchronizeControls()
        {
            _cardSelector.SetPlannedAmount(MinimumAmount);
            _amountStepper.ResetValue(GetMaximumAmount());
            _progressView.ClearPreview();
            RefreshUseButton();
        }

        private int GetMaximumAmount()
        {
            if (_cardSelector.HasSelection is false)
                return MinimumAmount;

            return Math.Min(
                _cardSelector.SelectedAmount,
                _progressView.GetMaximumApplicableAmount(_cardSelector.SelectedCard.ExperienceAmount));
        }

        private void RefreshUseButton()
        {
            _useButton.SetInteractable(CanUse());
        }

        private bool CanUse() =>
            _cardSelector.HasSelection &&
            _amountStepper.Value > MinimumAmount &&
            _amountStepper.Value <= _cardSelector.SelectedAmount &&
            _progressView.CanApplyPendingExperience;
    }

    internal sealed class UpgradeWindowParameters : IWindowParameters { }
}