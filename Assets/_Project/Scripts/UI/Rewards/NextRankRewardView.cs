using System;
using System.Collections.Generic;
using LL.Game.Heroes;
using LL.Game.RankUp;
using LL.Game.Ranks;
using LL.Game.Rewards;
using LL.Presentation.Localization;
using LL.Presentation.RankUp;
using R3;
using TMPro;
using UnityEngine;
using VContainer;

namespace LL.UI.Rewards
{
    [DisallowMultipleComponent]
    internal sealed class NextRankRewardView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _titleLabel;
        [SerializeField] private RewardContainerView _rewardContainer;

        private const string RankVariable = "rank";

        private RankCatalog _rankCatalog;
        private RankUpCatalog _rankUpCatalog;
        private RewardCatalog _rewardCatalog;
        private ILocalizationService _localization;
        private int _nextRank;
        private bool _hasPreview;

        [Inject]
        private void Construct(
            RankCatalog rankCatalog,
            RankUpCatalog rankUpCatalog,
            RewardCatalog rewardCatalog,
            ILocalizationService localization)
        {
            _rankCatalog = rankCatalog ?? throw new ArgumentNullException(nameof(rankCatalog));
            _rankUpCatalog = rankUpCatalog ?? throw new ArgumentNullException(nameof(rankUpCatalog));
            _rewardCatalog = rewardCatalog ?? throw new ArgumentNullException(nameof(rewardCatalog));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
        }

        private void Start()
        {
            _localization.LocaleChanged.Subscribe(_ => OnLocaleChanged()).AddTo(this);
        }

        internal void ShowNextRank(HeroId heroId, RankId currentRankId)
        {
            if (_rankCatalog.TryGetRank(currentRankId, out var currentRank) is false ||
                _rankUpCatalog.TryGetDefinition(heroId, currentRankId, out var definition) is false)
            {
                Clear();
                return;
            }

            if (_rewardCatalog.TryGetReward(definition.RewardId, out var reward) is false)
            {
                Debug.LogError($"Missing reward for rank-up: {definition.RewardId}", this);
                Clear();
                return;
            }

            _nextRank = currentRank.Number + 1;
            _hasPreview = true;
            gameObject.SetActive(true);
            RefreshTitle();
            _rewardContainer.SetItems(reward.Items);
        }

        internal void Clear()
        {
            _hasPreview = false;
            _titleLabel.text = string.Empty;
            _rewardContainer.Clear();
            gameObject.SetActive(false);
        }

        private void OnLocaleChanged()
        {
            if (_hasPreview)
                RefreshTitle();
        }

        private void RefreshTitle()
        {
            _titleLabel.text = _localization.GetText(
                RankUpLocalizationKeys.RewardsAtRank,
                new Dictionary<string, object>
                {
                    [RankVariable] = _nextRank
                });
        }
    }
}