using System;
using System.Collections.Generic;
using LL.Game.Promotions;
using LL.Presentation.Localization;
using LL.Presentation.Promotions;
using LL.Rewards;
using TMPro;
using UnityEngine;
using VContainer;

namespace LL.UI.Rewards
{
    [DisallowMultipleComponent]
    internal sealed class RewardPreview : MonoBehaviour
    {
        [SerializeField] private TMP_Text _titleLabel;
        [SerializeField] private RewardLayout _rewardLayout;

        private const string RankVariable = "rank";

        private RankPromotionCatalog _promotionCatalog;
        private RewardBundleCatalog _rewardBundleCatalog;
        private ILocalizationService _localization;

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

        internal void ShowNextRank(int currentRank)
        {
            if (_promotionCatalog.TryGetPromotion(currentRank, out var promotion) is false ||
                promotion.RewardBundleId.IsEmpty ||
                _rewardBundleCatalog.TryGetBundle(promotion.RewardBundleId, out var bundle) is false)
            {
                Clear();
                return;
            }

            gameObject.SetActive(true);
            _titleLabel.text = _localization.GetText(
                PromotionLocalizationKeys.RewardsAtRank,
                new Dictionary<string, object>
                {
                    [RankVariable] = currentRank + 1
                });
            _rewardLayout.SetRewards(bundle.Rewards);
        }

        internal void Clear()
        {
            _titleLabel.text = string.Empty;
            _rewardLayout.Clear();
            gameObject.SetActive(false);
        }
    }
}