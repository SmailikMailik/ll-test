using System;
using LL.Game.Ranks;
using LL.UI.Controls.Buttons;
using LL.UI.Controls.Steppers;
using LL.UI.Windows.Views.Upgrade.Cards;
using LL.UI.Windows.Views.Upgrade.Progress;
using LL.User.Core.Progress;
using LL.User.Core.Upgrades;
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

        private IUserProgress _userProgress;
        private ICardExperienceService _cardExperienceService;
        private IRankProgression _rankProgression;
        private UpgradeExperienceController _experienceController;

        private bool _isApplying;
        private bool _isInitialized;

        [Inject]
        private void Construct(
            IUserProgress userProgress,
            ICardExperienceService cardExperienceService,
            IRankProgression rankProgression)
        {
            _userProgress = userProgress ?? throw new ArgumentNullException(nameof(userProgress));
            _cardExperienceService = cardExperienceService ??
                throw new ArgumentNullException(nameof(cardExperienceService));
            _rankProgression = rankProgression ?? throw new ArgumentNullException(nameof(rankProgression));
        }

        protected override void OnShow()
        {
            InitializeComponents();
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
            _resetButton.Clicked
                .Subscribe(_ => ResetPendingChanges())
                .AddTo(this);
        }

        private void ResetPendingChanges()
        {
            _experienceController.ResetPreview();
            _cardSelector.Reset();
            RefreshUseButton();
        }

        private void OnCardSelectionChanged()
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
            _cardSelector.SetSelectedPlannedAmount(amount);
            _experienceController.SetPendingExperience(_cardSelector.PlannedExperience);
            RefreshUseButton();
        }

        private void ApplyCards()
        {
            if (CanUse() is false)
                return;

            _isApplying = true;

            try
            {
                _cardExperienceService.TryApply(_cardSelector.GetPlan());
            }
            finally
            {
                _isApplying = false;
                ResetPendingChanges();
            }
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

        private void RefreshUseButton()
        {
            _useButton.SetInteractable(CanUse());
        }

        private bool CanUse() =>
            _cardSelector.PlannedExperience > MinimumAmount &&
            _experienceController.CanApplyPendingExperience;
    }

    internal sealed class UpgradeWindowParameters : IWindowParameters { }
}