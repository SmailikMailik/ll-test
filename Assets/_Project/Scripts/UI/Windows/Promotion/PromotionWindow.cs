using System;
using System.Collections.Generic;
using LL.Game.Items;
using LL.Game.Payments;
using LL.Game.Promotions;
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

namespace LL.UI.Windows.Promotion
{
    internal sealed class PromotionWindow : Window<PromotionWindowParameters>
    {
        [SerializeField] private TMP_Text _currentRankLabel;
        [SerializeField] private TMP_Text _nextRankLabel;

        [SerializeField] private TMP_Text _questPriceLabel;

        [SerializeField] private InteractiveButton _questButton;

        [SerializeField] private TMP_Text _instantPriceLabel;

        [SerializeField] private InteractiveButton _instantButton;

        [SerializeField] private NextRankRewardView _nextRankRewardView;

        [SerializeField] private PromotionQuestView _questView;

        private IUserProgress _userProgress;
        private IRankProgression _rankProgression;
        private RankPromotionFlow _rankPromotionFlow;
        private QuestCatalog _questCatalog;
        private UpgradeFlow _upgradeFlow;
        private RankPromotion _promotion;

        [Inject]
        private void Construct(
            IUserProgress userProgress,
            IRankProgression rankProgression,
            RankPromotionFlow rankPromotionFlow,
            QuestCatalog questCatalog,
            UpgradeFlow upgradeFlow)
        {
            _userProgress = userProgress ?? throw new ArgumentNullException(nameof(userProgress));
            _rankProgression = rankProgression ?? throw new ArgumentNullException(nameof(rankProgression));
            _rankPromotionFlow = rankPromotionFlow ?? throw new ArgumentNullException(nameof(rankPromotionFlow));
            _questCatalog = questCatalog ?? throw new ArgumentNullException(nameof(questCatalog));
            _upgradeFlow = upgradeFlow ?? throw new ArgumentNullException(nameof(upgradeFlow));
        }

        private void Start()
        {
            _questButton.Clicked.Subscribe(_ => OnQuestPromotionClicked()).AddTo(this);
            _instantButton.Clicked.Subscribe(_ => OnInstantPromotionClicked()).AddTo(this);
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

            if (_rankPromotionFlow.TryGetPromotion(out _promotion) &&
                _questCatalog.TryGetQuest(_promotion.Quest.QuestId, out var quest))
            {
                _questPriceLabel.text = PaymentFormatter.Format(_promotion.Quest.Payment);
                _instantPriceLabel.text = PaymentFormatter.Format(_promotion.InstantPayment);
                _questView.Refresh(
                    _promotion.Quest,
                    quest,
                    _userProgress.CanPromoteRank);
            }
            else
            {
                _promotion = null;
                _questPriceLabel.text = string.Empty;
                _instantPriceLabel.text = string.Empty;
            }

            _questView.gameObject.SetActive(_promotion != null);
            RefreshActions();
        }

        private void PromoteRank(Payment payment)
        {
            if (_promotion == null || _userProgress.CanPromoteRank is false)
                return;

            _rankPromotionFlow.Promote(
                payment,
                OnPromotionSucceeded,
                OnPromotionFailed);
        }

        private void OnQuestPromotionClicked()
        {
            if (_promotion != null)
                PromoteRank(_promotion.Quest.Payment);
        }

        private void OnInstantPromotionClicked()
        {
            if (_promotion != null)
                PromoteRank(_promotion.InstantPayment);
        }

        private void OnPromotionSucceeded(IReadOnlyList<ItemAmount> rewardItems)
        {
            _upgradeFlow.CompletePromotion(_userProgress.Rank, rewardItems);
        }

        private void OnPromotionFailed()
        {
            RefreshActions();
        }

        private void OnQuestCompleted()
        {
            RefreshActions();
        }

        private void RefreshActions()
        {
            var hasPromotion = _promotion != null;
            var canPromoteRank = _userProgress.CanPromoteRank;

            _questView.SetAvailable(hasPromotion && canPromoteRank);
            _questButton.SetInteractable(
                hasPromotion &&
                canPromoteRank &&
                _questView.IsCompleted);
            _instantButton.SetInteractable(hasPromotion && canPromoteRank);
        }
    }
}