using System;
using System.Collections.Generic;
using LL.Game.Promotions;
using LL.Game.Rewards;
using LL.Presentation.Localization;
using LL.Presentation.Promotions;
using LL.UI.Rewards;
using R3;
using TMPro;
using UnityEngine;
using VContainer;

namespace LL.UI
{
    [DisallowMultipleComponent]
    internal sealed class NextRankRewardView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _titleLabel;
        [SerializeField] private RewardContainerView _rewardContainer;

        private const string RankVariable = "rank";

        private RankPromotionCatalog _promotionCatalog;
        private RewardBundleCatalog _rewardBundleCatalog;
        private ILocalizationService _localization;
        private int _nextRank;
        private bool _hasPreview;

        [Inject]
        private void Construct(
            RankPromotionCatalog promotionCatalog,
            RewardBundleCatalog rewardBundleCatalog,
            ILocalizationService localization)
        {
            _promotionCatalog = promotionCatalog ?? throw new ArgumentNullException(nameof(promotionCatalog));
            _rewardBundleCatalog = rewardBundleCatalog ?? throw new ArgumentNullException(nameof(rewardBundleCatalog));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
        }

        private void Start()
        {
            _localization.LocaleChanged.Subscribe(_ => OnLocaleChanged()).AddTo(this);
        }

        internal void ShowNextRank(int currentRank)
        {
            if (_promotionCatalog.TryGetPromotion(currentRank, out var promotion) is false)
            {
                Clear();
                return;
            }

            if (_rewardBundleCatalog.TryGetBundle(promotion.RewardBundleId, out var bundle) is false)
            {
                Debug.LogError($"Missing reward bundle for rank promotion: {promotion.RewardBundleId}", this);
                Clear();
                return;
            }

            _nextRank = currentRank + 1;
            _hasPreview = true;
            gameObject.SetActive(true);
            RefreshTitle();
            _rewardContainer.SetRewards(bundle.Rewards);
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
                RankPromotionLocalizationKeys.RewardsAtRank,
                new Dictionary<string, object>
                {
                    [RankVariable] = _nextRank
                });
        }
    }
}