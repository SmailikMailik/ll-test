using System;
using System.Collections.Generic;
using LL.Game.Items;
using LL.Game.Payments;
using LL.Game.RankUp;
using LL.Game.Quests;
using LL.Game.Ranks;
using LL.Presentation.Payments;
using LL.Presentation.Typography;
using LL.UI.Controls;
using LL.UI.Rewards;
using LL.UI.Windows.Flows;
using LL.User.State.Progress;
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

        private IUserProgress _userProgress;
        private IRankProgression _rankProgression;
        private RankUpFlow _rankUpFlow;
        private QuestCatalog _questCatalog;
        private UpgradeFlow _upgradeFlow;
        private RankUpDefinition _definition;

        [Inject]
        private void Construct(
            IUserProgress userProgress,
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
            var progress = _rankProgression.GetProgress(_userProgress.RankId, _userProgress.Experience);
            var rank = progress.Rank;
            var nextRank = progress.HasNextRank ? rank + 1 : rank;

            _currentRankLabel.text = TextFormatter.Number(rank);
            _nextRankLabel.text = TextFormatter.Number(nextRank);
            _nextRankRewardView.ShowNextRank(_userProgress.RankId);

            if (_rankUpFlow.TryGetDefinition(out _definition) &&
                _questCatalog.TryGetQuest(_definition.Quest.QuestId, out var quest))
            {
                _questPriceLabel.text = PaymentFormatter.Format(_definition.Quest.Payment);
                _instantPriceLabel.text = PaymentFormatter.Format(_definition.InstantPayment);
                _questView.Refresh(
                    _definition.Quest,
                    quest,
                    _userProgress.CanRankUp);
            }
            else
            {
                _definition = null;
                _questPriceLabel.text = string.Empty;
                _instantPriceLabel.text = string.Empty;
            }

            _questView.gameObject.SetActive(_definition != null);
            RefreshActions();
        }

        private void RequestRankUp(Payment payment)
        {
            if (_definition == null || _userProgress.CanRankUp is false)
                return;

            _rankUpFlow.RequestRankUp(
                payment,
                OnRankUpSucceeded,
                OnRankUpFailed);
        }

        private void OnQuestRankUpClicked()
        {
            if (_definition != null)
                RequestRankUp(_definition.Quest.Payment);
        }

        private void OnInstantRankUpClicked()
        {
            if (_definition != null)
                RequestRankUp(_definition.InstantPayment);
        }

        private void OnRankUpSucceeded(IReadOnlyList<ItemAmount> rewardItems)
        {
            _upgradeFlow.CompleteRankUp(_userProgress.Rank, rewardItems);
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
            var hasRankUp = _definition != null;
            var canRankUp = _userProgress.CanRankUp;

            _questView.SetAvailable(hasRankUp && canRankUp);
            _questButton.SetInteractable(
                hasRankUp &&
                canRankUp &&
                _questView.IsCompleted);
            _instantButton.SetInteractable(hasRankUp && canRankUp);
        }
    }
}