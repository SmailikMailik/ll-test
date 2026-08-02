using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Items;
using LL.Game.Payments;
using LL.Game.Quests;
using LL.Game.Ranks;
using LL.Game.RankUp;
using LL.Presentation.Payments;
using LL.Presentation.Typography;
using LL.UI.Controls;
using LL.UI.Rewards;
using LL.UI.Windows.Flows;
using LL.User.State.Heroes;
using R3;
using TMPro;
using UnityEngine;
using VContainer;

namespace LL.UI.Windows.RankUp
{
    internal sealed class RankUpWindow : Window<RankUpWindowParameters>
    {
        [SerializeField] private TMP_Text _currentRankLabel;
        [SerializeField] private TMP_Text _nextRankLabel;

        [SerializeField] private TMP_Text _questPriceLabel;
        [SerializeField] private InteractiveButton _questButton;

        [SerializeField] private TMP_Text _instantPriceLabel;
        [SerializeField] private InteractiveButton _instantButton;

        [SerializeField] private NextRankRewardView _nextRankRewardView;
        [SerializeField] private RankUpQuestView _questView;

        private IUserHeroProgress _userProgress;
        private IRankProgression _rankProgression;
        private RankUpFlow _rankUpFlow;
        private QuestCatalog _questCatalog;
        private UpgradeFlow _upgradeFlow;
        private RankUpDefinition _definition;
        private RankUpOptionDefinition _questOption;
        private RankUpOptionDefinition _instantOption;
        private QuestRankUpRequirementDefinition _questRequirement;

        [Inject]
        private void Construct(
            IUserHeroProgress userProgress,
            IRankProgression rankProgression,
            RankUpFlow rankUpFlow,
            QuestCatalog questCatalog,
            UpgradeFlow upgradeFlow)
        {
            _userProgress = userProgress ?? throw new ArgumentNullException(nameof(userProgress));
            _rankProgression = rankProgression ?? throw new ArgumentNullException(nameof(rankProgression));
            _rankUpFlow = rankUpFlow ?? throw new ArgumentNullException(nameof(rankUpFlow));
            _questCatalog = questCatalog ?? throw new ArgumentNullException(nameof(questCatalog));
            _upgradeFlow = upgradeFlow ?? throw new ArgumentNullException(nameof(upgradeFlow));
        }

        private void Start()
        {
            _questButton.Clicked.Subscribe(_ => OnQuestRankUpClicked()).AddTo(this);
            _instantButton.Clicked.Subscribe(_ => OnInstantRankUpClicked()).AddTo(this);
            _questView.Completed.Subscribe(_ => OnQuestCompleted()).AddTo(this);
        }

        protected override void OnShow()
        {
            if (_userProgress.TryGetProgress(Parameters.HeroId, out var userProgress) is false)
            {
                ClearDefinition();
                return;
            }

            var progress = _rankProgression.GetProgress(userProgress.RankId, userProgress.Experience);
            var rankNumber = progress.RankNumber;
            var nextRankNumber = progress.HasNextRank ? rankNumber + 1 : rankNumber;

            _currentRankLabel.text = TextFormatter.Number(rankNumber);
            _nextRankLabel.text = TextFormatter.Number(nextRankNumber);
            _nextRankRewardView.ShowNextRank(Parameters.HeroId, userProgress.RankId);

            if (_rankUpFlow.TryGetDefinition(Parameters.HeroId, out _definition) is false)
            {
                ClearDefinition();
                return;
            }

            _questOption = _definition.Options.FirstOrDefault(option =>
                option.Requirements.OfType<QuestRankUpRequirementDefinition>().Any());
            _instantOption = _definition.Options.FirstOrDefault(option =>
                ReferenceEquals(option, _questOption) is false);

            _questRequirement = _questOption?.Requirements.OfType<QuestRankUpRequirementDefinition>().FirstOrDefault();

            RefreshOptionPrice(_questOption, _questPriceLabel);
            RefreshOptionPrice(_instantOption, _instantPriceLabel);

            if (_questRequirement is not null &&
                _questCatalog.TryGetQuest(_questRequirement.QuestId, out var quest))
            {
                _questView.Refresh(
                    Parameters.HeroId,
                    _definition.RankId,
                    _questOption.Id,
                    _questRequirement,
                    quest,
                    _userProgress.CanRankUp(Parameters.HeroId));
            }

            _questView.gameObject.SetActive(_questRequirement is not null);
            _questButton.gameObject.SetActive(_questOption is not null);
            _instantButton.gameObject.SetActive(_instantOption is not null);
            RefreshActions();
        }

        private void RequestRankUp(RankUpOptionDefinition option)
        {
            if (_definition is null ||
                option is null ||
                _userProgress.CanRankUp(Parameters.HeroId) is false)
            {
                return;
            }

            _rankUpFlow.RequestRankUp(
                Parameters.HeroId,
                option.Id,
                OnRankUpSucceeded,
                OnRankUpFailed);
        }

        private void OnQuestRankUpClicked()
        {
            RequestRankUp(_questOption);
        }

        private void OnInstantRankUpClicked()
        {
            RequestRankUp(_instantOption);
        }

        private void OnRankUpSucceeded(IReadOnlyList<ItemAmount> rewardItems)
        {
            _upgradeFlow.CompleteRankUp(
                _userProgress.GetRankNumber(Parameters.HeroId),
                rewardItems);
        }

        private void OnRankUpFailed()
        {
            RefreshActions();
        }

        private void OnQuestCompleted()
        {
            RefreshActions();
        }

        private void RefreshActions()
        {
            var canRankUp = _definition is not null && _userProgress.CanRankUp(Parameters.HeroId);

            _questView.SetAvailable(canRankUp && _questOption is not null);
            _questButton.SetInteractable(
                canRankUp &&
                _questOption is not null &&
                _questView.IsCompleted);
            _instantButton.SetInteractable(canRankUp && _instantOption is not null);
        }

        private void ClearDefinition()
        {
            _definition = null;
            _questOption = null;
            _instantOption = null;
            _questRequirement = null;
            _questPriceLabel.text = string.Empty;
            _instantPriceLabel.text = string.Empty;
            _questView.gameObject.SetActive(false);
            _questButton.gameObject.SetActive(false);
            _instantButton.gameObject.SetActive(false);
        }

        private static void RefreshOptionPrice(
            RankUpOptionDefinition option,
            TMP_Text label)
        {
            var payment = option?.Requirements
                .OfType<PaymentRankUpRequirementDefinition>()
                .Select(requirement => (Payment?)requirement.Payment)
                .FirstOrDefault();
            label.text = payment.HasValue ? PaymentFormatter.Format(payment.Value) : string.Empty;
        }
    }
}