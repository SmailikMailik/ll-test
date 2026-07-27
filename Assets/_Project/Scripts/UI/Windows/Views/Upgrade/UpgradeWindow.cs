using System;
using LL.Game.Ranks;
using LL.UI.Controls.Buttons;
using LL.UI.Controls.Steppers;
using LL.UI.Windows.Views.Upgrade.Cards;
using LL.UI.Windows.Views.Upgrade.Progress;
using LL.User.Core.Cards;
using LL.User.Core.Progress;
using R3;
using UnityEngine;
using VContainer;

namespace LL.UI.Windows.Views.Upgrade
{
    internal sealed class UpgradeWindow : Window<UpgradeWindowParameters>
    {
        [SerializeField] private UpgradeExperienceView _experienceView;
        [SerializeField] private UpgradeCardSelector _cardSelector;
        [SerializeField] private AmountStepper _amountStepper;

        [SerializeField] private InteractiveButton _useButton;
        [SerializeField] private InteractiveButton _resetButton;

        private const int AmountDelta = 1;
        private const int MinimumAmount = 0;

        private IUserCards _userCards;
        private IUserProgress _userProgress;
        private IRankProgression _rankProgression;
        private UpgradeExperienceController _experienceController;

        private bool _isApplying;
        private bool _isInitialized;

        [Inject]
        private void Construct(
            IUserCards userCards,
            IUserProgress userProgress,
            IRankProgression rankProgression)
        {
            _userCards = userCards ?? throw new ArgumentNullException(nameof(userCards));
            _userProgress = userProgress ?? throw new ArgumentNullException(nameof(userProgress));
            _rankProgression = rankProgression ?? throw new ArgumentNullException(nameof(rankProgression));
        }

        protected override void OnShow()
        {
            InitializeComponents();
            _experienceController.ResetPreview();
            ResetPendingChanges();
        }

        protected override void OnHide()
        {
            _cardSelector.SetPlannedAmount(MinimumAmount);
            _amountStepper.ResetValue();
            _experienceController.ClearPreview();
        }

        private void InitializeComponents()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;
            _experienceController = new UpgradeExperienceController(
                _experienceView,
                _userProgress,
                _rankProgression);
            _cardSelector.Initialize();
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
            _resetButton.Clicked
                .Subscribe(_ => ResetPendingChanges())
                .AddTo(this);
        }

        private void ResetPendingChanges()
        {
            _experienceController.ClearPreview();
            _amountStepper.ResetValue();
            _cardSelector.ResetSelection();
            RefreshUseButton();
        }

        private void OnSelectionChanged()
        {
            if (_isApplying)
                return;

            _cardSelector.SetPlannedAmount(MinimumAmount);
            _experienceController.ClearPreview();
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
                _experienceController.SetPendingItems(amount, _cardSelector.SelectedCard.ExperienceAmount);
            else
                _experienceController.ClearPreview();

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

            if (_experienceController.TryApplyPendingExperience() is false)
                _userCards.TryAdd(cardId, amount);

            _isApplying = false;
            SynchronizeControls();
        }

        private void SynchronizeControls()
        {
            _cardSelector.SetPlannedAmount(MinimumAmount);
            _amountStepper.ResetValue(GetMaximumAmount());
            _experienceController.ClearPreview();
            RefreshUseButton();
        }

        private int GetMaximumAmount()
        {
            if (_cardSelector.HasSelection is false)
                return MinimumAmount;

            return Math.Min(
                _cardSelector.SelectedAmount,
                _experienceController.GetMaximumApplicableAmount(_cardSelector.SelectedCard.ExperienceAmount));
        }

        private void RefreshUseButton()
        {
            _useButton.SetInteractable(CanUse());
        }

        private bool CanUse() =>
            _cardSelector.HasSelection &&
            _amountStepper.Value > MinimumAmount &&
            _amountStepper.Value <= _cardSelector.SelectedAmount &&
            _experienceController.CanApplyPendingExperience;
    }

    internal sealed class UpgradeWindowParameters : IWindowParameters { }
}