using System;
using System.Collections.Generic;
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
            _cardSelector.ClearPlannedAmounts();
            _amountStepper.SetValue(MinimumAmount, MinimumAmount);
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
            _cardSelector.ResetSelection();
            _experienceController.ClearPreview();
            RefreshUseButton();
        }

        private void OnSelectionChanged()
        {
            if (_isApplying)
                return;

            _amountStepper.SetValue(
                _cardSelector.SelectedPlannedAmount,
                GetMaximumAmount());
            RefreshUseButton();
        }

        private void OnSelectedAmountChanged()
        {
            if (_isApplying)
                return;

            _amountStepper.SetValue(
                _cardSelector.SelectedPlannedAmount,
                GetMaximumAmount());
            RefreshUseButton();
        }

        private void OnPlannedAmountChanged(int amount)
        {
            _cardSelector.SetPlannedAmount(amount);
            _experienceController.SetPendingExperience(_cardSelector.PlannedExperience);
            RefreshUseButton();
        }

        private void ApplyCards()
        {
            if (CanUse() is false)
                return;

            var plannedCards = _cardSelector.GetPlannedCards();
            var spentCards = new List<PlannedCard>(plannedCards.Count);

            _isApplying = true;

            foreach (var plannedCard in plannedCards)
            {
                if (_userCards.TrySpend(plannedCard.Id, plannedCard.Amount))
                {
                    spentCards.Add(plannedCard);
                    continue;
                }

                RestoreCards(spentCards);
                _isApplying = false;
                SynchronizeControls();
                return;
            }

            if (_experienceController.TryApplyPendingExperience() is false)
                RestoreCards(spentCards);

            _isApplying = false;
            SynchronizeControls();
        }

        private void SynchronizeControls()
        {
            ResetPendingChanges();
        }

        private int GetMaximumAmount()
        {
            if (_cardSelector.HasSelection is false)
                return MinimumAmount;

            var experiencePerItem = _cardSelector.SelectedCard.ExperienceAmount;
            var selectedExperience = _cardSelector.SelectedPlannedAmount * experiencePerItem;
            var reservedExperience = _cardSelector.PlannedExperience - selectedExperience;
            var maximumExperienceAmount = _experienceController.GetMaximumApplicableAmount(
                experiencePerItem,
                reservedExperience);

            return Math.Min(_cardSelector.SelectedAmount, maximumExperienceAmount);
        }

        private void RefreshUseButton()
        {
            _useButton.SetInteractable(CanUse());
        }

        private bool CanUse() =>
            _cardSelector.PlannedExperience > MinimumAmount &&
            _experienceController.CanApplyPendingExperience;

        private void RestoreCards(IEnumerable<PlannedCard> cards)
        {
            foreach (var card in cards)
                _userCards.TryAdd(card.Id, card.Amount);
        }
    }

    internal sealed class UpgradeWindowParameters : IWindowParameters { }
}