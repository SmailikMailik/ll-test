using System;
using System.Linq;
using LL.Game.Promotions;
using LL.Loading;
using LL.Rewards;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Game.Configuration
{
    [CreateAssetMenu(fileName = nameof(RankPromotionCatalogConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class RankPromotionCatalogConfig : ScriptableObject, IDataLoader<RankPromotionCatalog>
    {
        [ValidateInput(nameof(HasValidPromotions), "Promotion ranks must be positive and unique.")]
        [ListDrawerSettings(ShowFoldout = false, ShowPaging = false)]
        [SerializeField] private RankPromotionEntry[] _promotions;

        internal const string CreationPath = "LL/Game Data/Rank Promotion Catalog";

        public RankPromotionCatalog Load()
        {
            return new RankPromotionCatalog(_promotions?.Select(entry => entry?.ToPromotion()));
        }

        private void OnValidate()
        {
            if (_promotions == null)
                return;

            foreach (var promotion in _promotions)
                promotion?.Normalize();
        }

        private static bool HasValidPromotions(RankPromotionEntry[] promotions)
        {
            return RankPromotionCatalogValidator.HasValidRanks(
                promotions,
                promotion => promotion.Rank);
        }
    }

    [Serializable]
    internal sealed class RankPromotionEntry
    {
        [HorizontalGroup("Columns")]
        [BoxGroup("Columns/Promotion")]
        [MinValue(1)]
        [SerializeField] private int _rank = 1;

        [BoxGroup("Columns/Promotion")]
        [LabelText("Duration")]
        [SuffixLabel("min", true)]
        [MinValue(1)]
        [SerializeField] private int _durationMinutes = 1440;

        [BoxGroup("Columns/Requirement")]
        [LabelText("ID")]
        [SerializeField] private string _requirementId;

        [BoxGroup("Columns/Requirement")]
        [LabelText("Amount")]
        [MinValue(1)]
        [SerializeField] private int _requiredAmount = 1;

        [BoxGroup("Columns/Localization")]
        [LabelText("Title")]
        [SerializeField] private string _titleLocalizationKey;

        [BoxGroup("Columns/Localization")]
        [LabelText("Description")]
        [SerializeField] private string _descriptionLocalizationKey;

        [BoxGroup("Columns/Localization")]
        [LabelText("Target")]
        [SerializeField] private string _targetLocalizationKey;

        [BoxGroup("Columns/Economy")]
        [LabelText("Soft")]
        [MinValue(1)]
        [SerializeField] private int _softPrice = 1;

        [BoxGroup("Columns/Economy")]
        [LabelText("Hard")]
        [MinValue(1)]
        [SerializeField] private int _hardPrice = 1;

        [BoxGroup("Columns/Economy")]
        [LabelText("Reward")]
        [SerializeField] private string _rewardBundleId;

        internal int Rank => _rank;

        internal RankPromotion ToPromotion()
        {
            return new RankPromotion(
                _rank,
                new RankPromotionRequirement(
                    new PromotionRequirementId(_requirementId),
                    _titleLocalizationKey,
                    _descriptionLocalizationKey,
                    _targetLocalizationKey,
                    _requiredAmount),
                TimeSpan.FromMinutes(_durationMinutes),
                _softPrice,
                _hardPrice,
                new RewardBundleId(_rewardBundleId));
        }

        internal void Normalize()
        {
            _rank = Math.Max(1, _rank);
            _durationMinutes = Math.Max(1, _durationMinutes);
            _requiredAmount = Math.Max(1, _requiredAmount);
            _softPrice = Math.Max(1, _softPrice);
            _hardPrice = Math.Max(1, _hardPrice);
        }
    }
}