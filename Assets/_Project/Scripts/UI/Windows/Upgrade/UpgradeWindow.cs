using System;
using System.Collections.Generic;
using LL.Game.Items;
using LL.Game.Ranks;
using LL.Game.Upgrades.Services;
using LL.Presentation.Upgrades;
using LL.UI.Controls;
using LL.UI.Rewards;
using LL.UI.Windows.Flows;
using LL.UI.Windows.Upgrade.Cards;
using LL.UI.Windows.Upgrade.Progress;
using LL.User.State.Heroes;
using R3;
using UnityEngine;
using VContainer;

namespace LL.UI.Windows.Upgrade
{
    internal sealed class UpgradeWindow : Window<UpgradeWindowParameters>
    {
        [SerializeField] private UpgradeExperienceView _experienceView;
        [SerializeField] private NextRankRewardView _nextRankRewardView;
        [SerializeField] private UpgradeCardSelector _cardSelector;
        [SerializeField] private AmountStepper _amountStepper;

        [SerializeField] private InteractiveButton _useButton;
        [SerializeField] private InteractiveButton _maxButton;
        [SerializeField] private InteractiveButton _resetButton;

        private const int AmountDelta = 1;
        private const int MinAmount = 0;

        private IUserHeroProgress _userProgress;
        private ICardExperienceService _cardExperienceService;
        private IExperienceOverflowConfirmation _overflowConfirmation;
        private IRankProgression _rankProgression;
        private UpgradeFlow _upgradeFlow;
        private UpgradeExperienceController _experienceController;

        private bool _isApplying;
        private bool _isInitialized;

        [Inject]
        private void Construct(
            IUserHeroProgress userProgress,
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
            if (_userProgress.TryGetProgress(Parameters.HeroId, out var progress) is false)
                return;

            _experienceController = new UpgradeExperienceController(
                _experienceView,
                Parameters.HeroId,
                _userProgress,
                _rankProgression);
            InitializeComponents();
            _nextRankRewardView.ShowNextRank(Parameters.HeroId, progress.RankId);
            ResetPendingChanges();
        }

        protected override void OnHide()
        {
            _cardSelector.ClearPlan();
            _amountStepper.SetValue(MinAmount, MinAmount);
            _experienceController?.ClearPreview();
        }

        private void InitializeComponents()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;
            _cardSelector.Initialize();
            _amountStepper.Initialize(AmountDelta);

            _cardSelector.Changed.Subscribe(_ => OnCardSelectionChanged()).AddTo(this);
            _amountStepper.ValueChanged.Subscribe(OnPlannedAmountChanged).AddTo(this);
            _useButton.Clicked.Subscribe(_ => ApplyCards()).AddTo(this);
            _maxButton.Clicked.Subscribe(_ => SetMaxPlan()).AddTo(this);
            _resetButton.Clicked.Subscribe(_ => ResetPendingChanges()).AddTo(this);
        }

        private void ResetPendingChanges()
        {
            _experienceController.ResetPreview();
            _cardSelector.RestoreDefaultSelection();
            RefreshActions();
        }

        private void OnCardSelectionChanged()
        {
            if (_isApplying)
                return;

            _amountStepper.SetValue(_cardSelector.SelectedPlannedAmount, GetMaxAmount());
            _experienceController.SetPendingExperience(_cardSelector.PlannedExperience);
            RefreshActions();
        }

        private void OnPlannedAmountChanged(int amount)
        {
            _cardSelector.SetSelectedPlannedAmount(amount);
            _experienceController.SetPendingExperience(_cardSelector.PlannedExperience);
            RefreshActions();
        }

        private void SetMaxPlan()
        {
            if (_cardSelector.PlannedExperience > MinAmount)
                return;

            _cardSelector.SetMaxPlan(_experienceController.RemainingExperience);
        }

        private void ApplyCards()
        {
            if (CanUse() is false)
                return;

            var plan = _cardSelector.GetPlan();

            if (_cardExperienceService.TryGetApplication(Parameters.HeroId, plan, out var application) is false)
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

        private void ApplyCards(IReadOnlyList<ItemAmount> cards)
        {
            _isApplying = true;
            bool applied;

            try
            {
                applied = _cardExperienceService.TryApply(Parameters.HeroId, cards);
            }
            finally
            {
                _isApplying = false;
                ResetPendingChanges();
            }

            if (applied && _userProgress.CanRankUp(Parameters.HeroId))
                _upgradeFlow.ReplaceCurrent();
        }

        private int GetMaxAmount()
        {
            if (_cardSelector.HasSelection is false)
                return MinAmount;

            var experiencePerItem = _cardSelector.SelectedCard.ExperienceAmount;
            var selectedExperience = _cardSelector.SelectedPlannedAmount * experiencePerItem;
            var reservedExperience = _cardSelector.PlannedExperience - selectedExperience;
            var maxExperienceAmount = _experienceController.GetMaxApplicableAmount(
                experiencePerItem,
                reservedExperience);

            return Math.Min(_cardSelector.SelectedAvailableAmount, maxExperienceAmount);
        }

        private void RefreshActions()
        {
            var hasPlan = _cardSelector.PlannedExperience > MinAmount;

            _maxButton.gameObject.SetActive(hasPlan is false);
            _resetButton.gameObject.SetActive(hasPlan);

            _maxButton.SetInteractable(
                hasPlan is false &&
                _cardSelector.CanReachExperience(_experienceController.RemainingExperience));
            _useButton.SetInteractable(CanUse());
        }

        private bool CanUse() =>
            _cardSelector.PlannedExperience > MinAmount &&
            _experienceController.CanApplyPendingExperience;
    }
}