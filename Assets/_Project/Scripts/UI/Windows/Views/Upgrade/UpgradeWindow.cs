using System;
using System.Collections.Generic;
using LL.Game.Ranks;
using LL.Upgrades;
using LL.UI.Controls.Buttons;
using LL.UI.Controls.Steppers;
using LL.UI.Rewards;
using LL.UI.Windows.Flows;
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
        [SerializeField] private RewardPreview _rewardPreview;
        [SerializeField] private UpgradeCardSelector _cardSelector;
        [SerializeField] private AmountStepper _amountStepper;

        [SerializeField] private InteractiveButton _useButton;
        [SerializeField] private InteractiveButton _maxButton;
        [SerializeField] private InteractiveButton _resetButton;

        private const int AmountDelta = 1;
        private const int MinimumAmount = 0;

        private IUserProgress _userProgress;
        private ICardExperienceService _cardExperienceService;
        private IExperienceOverflowConfirmation _overflowConfirmation;
        private IRankProgression _rankProgression;
        private UpgradeFlow _upgradeFlow;
        private UpgradeExperienceController _experienceController;

        private bool _isApplying;
        private bool _isInitialized;

        [Inject]
        private void Construct(
            IUserProgress userProgress,
            ICardExperienceService cardExperienceService,
            IExperienceOverflowConfirmation overflowConfirmation,
            IRankProgression rankProgression,
            UpgradeFlow upgradeFlow)
        {
            _userProgress = userProgress ?? throw new ArgumentNullException(nameof(userProgress));
            _cardExperienceService = cardExperienceService ?? throw new ArgumentNullException(nameof(cardExperienceService));
            _overflowConfirmation = overflowConfirmation ?? throw new ArgumentNullException(nameof(overflowConfirmation));
            _rankProgression = rankProgression ?? throw new ArgumentNullException(nameof(rankProgression));
            _upgradeFlow = upgradeFlow ?? throw new ArgumentNullException(nameof(upgradeFlow));
        }

        protected override void OnShow()
        {
            InitializeComponents();
            _rewardPreview.ShowNextRank(_userProgress.Rank);
            ResetPendingChanges();
        }

        protected override void OnHide()
        {
            _cardSelector.ClearPlan();
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

            _cardSelector.Changed
                .Subscribe(_ => OnCardSelectionChanged())
                .AddTo(this);
            _amountStepper.ValueChanged
                .Subscribe(OnPlannedAmountChanged)
                .AddTo(this);
            _useButton.Clicked
                .Subscribe(_ => ApplyCards())
                .AddTo(this);
            _maxButton.Clicked
                .Subscribe(_ => SetMaximumPlan())
                .AddTo(this);
            _resetButton.Clicked
                .Subscribe(_ => ResetPendingChanges())
                .AddTo(this);
        }

        private void ResetPendingChanges()
        {
            _experienceController.ResetPreview();
            _cardSelector.Reset();
            RefreshActions();
        }

        private void OnCardSelectionChanged()
        {
            if (_isApplying)
                return;

            _amountStepper.SetValue(_cardSelector.SelectedPlannedAmount, GetMaximumAmount());
            _experienceController.SetPendingExperience(_cardSelector.PlannedExperience);
            RefreshActions();
        }

        private void OnPlannedAmountChanged(int amount)
        {
            _cardSelector.SetSelectedPlannedAmount(amount);
            _experienceController.SetPendingExperience(_cardSelector.PlannedExperience);
            RefreshActions();
        }

        private void SetMaximumPlan()
        {
            if (_cardSelector.PlannedExperience > MinimumAmount)
                return;

            _cardSelector.SetMaximumPlan(_experienceController.RemainingExperience);
        }

        private void ApplyCards()
        {
            if (CanUse() is false)
                return;

            var plan = _cardSelector.GetPlan();

            if (_cardExperienceService.TryGetApplication(plan, out var application) is false)
                return;

            if (application.HasLoss)
            {
                _overflowConfirmation.Confirm(
                    application.LostExperience,
                    () => ApplyCards(plan),
                    RefreshActions);
                return;
            }

            ApplyCards(plan);
        }

        private void ApplyCards(IReadOnlyList<CardStack> cards)
        {
            _isApplying = true;
            bool applied;

            try
            {
                applied = _cardExperienceService.TryApply(cards);
            }
            finally
            {
                _isApplying = false;
                ResetPendingChanges();
            }

            if (applied && _userProgress.CanPromoteRank)
                _upgradeFlow.ReplaceCurrent();
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

            return Math.Min(_cardSelector.SelectedAvailableAmount, maximumExperienceAmount);
        }

        private void RefreshActions()
        {
            var hasPlan = _cardSelector.PlannedExperience > MinimumAmount;

            _maxButton.gameObject.SetActive(hasPlan is false);
            _resetButton.gameObject.SetActive(hasPlan);

            _maxButton.SetInteractable(
                hasPlan is false &&
                _cardSelector.CanReachExperience(_experienceController.RemainingExperience));
            _useButton.SetInteractable(CanUse());
        }

        private bool CanUse() =>
            _cardSelector.PlannedExperience > MinimumAmount &&
            _experienceController.CanApplyPendingExperience;
    }

    internal sealed class UpgradeWindowParameters : IWindowParameters { }
}