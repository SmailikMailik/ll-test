using System;
using System.Collections.Generic;
using LL.Game.Payments.Configuration;
using LL.Game.Quests;
using LL.Game.Ranks;
using LL.Game.Rewards;
using LL.Validation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Game.Promotions.Configuration
{
    [CreateAssetMenu(fileName = nameof(RankPromotionCatalogConfig), menuName = CreationPath)]
    internal sealed class RankPromotionCatalogConfig : ScriptableObject, IValidationSource
    {
        [ValidateInput(nameof(HasValidPromotions), "Rank promotion data is invalid.")]
        [SerializeField] private RankPromotionEntry[] _promotions;

        internal const string CreationPath = "LL/Game Data/Rank Promotion Catalog";

        private static readonly IDataValidator<RankPromotionEntry[]> _validator = new RankPromotionCatalogConfigValidator();

        internal IReadOnlyList<RankPromotionEntry> Promotions => _promotions;

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
        [SerializeField] private string _rankId;
        [SerializeField] private string _questId;
        [SerializeField] private string _heroLocalizationKey;

        [SerializeField, MinValue(1)] private int _requiredAmount = 1;
        [SerializeField, MinValue(1)] private int _durationMinutes = 1440;

        [SerializeField] private PaymentEntry _questPayment = new();
        [SerializeField] private PaymentEntry _instantPayment = new();
        [SerializeField] private string _rewardId;

        internal RankId RankId => new(_rankId);
        internal QuestId QuestId => new(_questId);
        internal string HeroLocalizationKey => _heroLocalizationKey;
        internal int RequiredAmount => _requiredAmount;
        internal int DurationMinutes => _durationMinutes;
        internal PaymentEntry QuestPayment => _questPayment;
        internal PaymentEntry InstantPayment => _instantPayment;
        internal RewardId RewardId => new(_rewardId);
    }
}