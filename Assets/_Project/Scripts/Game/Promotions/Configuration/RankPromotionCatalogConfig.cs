using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Items;
using LL.Game.Payments;
using LL.Game.Rewards;
using LL.Infrastructure.Loading;
using LL.Validation;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace LL.Game.Promotions.Configuration
{
    [CreateAssetMenu(fileName = nameof(RankPromotionCatalogConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class RankPromotionCatalogConfig :
        ScriptableObject,
        IDataLoader<RankPromotionCatalog>,
        IValidationSource
    {
        [ValidateInput(nameof(HasValidPromotions), "Rank promotion data is invalid.")]
        [ListDrawerSettings(ShowFoldout = false, ShowPaging = false)]
        [SerializeField] private RankPromotionEntry[] _promotions;

        internal const string CreationPath = "LL/Game Data/Rank Promotion Catalog";

        private static readonly IDataValidator<RankPromotionEntry[]> _validator =
            new RankPromotionCatalogConfigValidator();

        internal IReadOnlyList<RankPromotionEntry> Promotions => _promotions;

        public RankPromotionCatalog Load()
        {
            ValidationRunner.EnsureValid(this, nameof(_promotions));

            return new RankPromotionCatalog(_promotions.Select(entry => entry.ToPromotion()));
        }

        private static bool HasValidPromotions(RankPromotionEntry[] promotions)
        {
            return ValidationRunner.IsValid(promotions, _validator);
        }

        void IValidationSource.Validate(ValidationContext context)
        {
            _validator.Validate(_promotions, context);
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
        [FormerlySerializedAs("_rewardBundleId")]
        [SerializeField] private string _rewardId;

        internal int Rank => _rank;
        internal int DurationMinutes => _durationMinutes;
        internal PromotionRequirementId RequirementId => new(_requirementId);
        internal int RequiredAmount => _requiredAmount;
        internal string TitleLocalizationKey => _titleLocalizationKey;
        internal string DescriptionLocalizationKey => _descriptionLocalizationKey;
        internal string TargetLocalizationKey => _targetLocalizationKey;
        internal int SoftPrice => _softPrice;
        internal int HardPrice => _hardPrice;
        internal RewardId RewardId => new(_rewardId);

        internal RankPromotion ToPromotion()
        {
            return new RankPromotion(
                Rank,
                new RankPromotionRequirement(
                    RequirementId,
                    TitleLocalizationKey,
                    DescriptionLocalizationKey,
                    TargetLocalizationKey,
                    RequiredAmount),
                TimeSpan.FromMinutes(DurationMinutes),
                new Payment(ItemIds.Soft, SoftPrice),
                new Payment(ItemIds.Hard, HardPrice),
                RewardId);
        }
    }
}