using System;
using System.Collections.Generic;
using LL.Game.Heroes;
using LL.Game.Payments.Configuration;
using LL.Game.Quests;
using LL.Game.Ranks;
using LL.Game.Rewards;
using LL.Validation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Game.RankUp.Configuration
{
    [CreateAssetMenu(fileName = nameof(RankUpCatalogConfig), menuName = CreationPath)]
    internal sealed class RankUpCatalogConfig : ScriptableObject, IValidationSource
    {
        [ValidateInput(nameof(HasValidRankUps), "Rank-up data is invalid.")]
        [SerializeField] private RankUpEntry[] _rankUps;

        internal const string CreationPath = "LL/Game Data/Rank-Up Catalog";

        private static readonly IDataValidator<RankUpEntry[]> _validator = new RankUpCatalogConfigValidator();

        internal IReadOnlyList<RankUpEntry> RankUps => _rankUps;

        private static bool HasValidRankUps(RankUpEntry[] rankUps)
        {
            return ValidationRunner.IsValid(rankUps, _validator);
        }

        void IValidationSource.Validate(ValidationContext context)
        {
            _validator.Validate(_rankUps, context);
        }
    }

    [Serializable]
    internal sealed class RankUpEntry
    {
        [SerializeField] private string _heroId;
        [SerializeField] private string _rankId;
        [SerializeField] private string _rewardId;
        [SerializeField] private RankUpOptionEntry[] _options;

        internal HeroId HeroId => new(_heroId);
        internal RankId RankId => new(_rankId);
        internal RewardId RewardId => new(_rewardId);
        internal IReadOnlyList<RankUpOptionEntry> Options => _options;
    }

    [Serializable]
    internal sealed class RankUpOptionEntry
    {
        [SerializeField] private string _optionId;
        [SerializeReference] private RankUpRequirementEntry[] _requirements;

        internal RankUpOptionId OptionId => new(_optionId);
        internal IReadOnlyList<RankUpRequirementEntry> Requirements => _requirements;
    }

    [Serializable]
    internal abstract class RankUpRequirementEntry
    {
        [SerializeField] private string _requirementId;

        internal RankUpRequirementId RequirementId => new(_requirementId);
    }

    [Serializable]
    internal sealed class QuestRankUpRequirementEntry : RankUpRequirementEntry
    {
        [SerializeField] private string _questId;

        [MinValue(1)]
        [SerializeField] private int _requiredCount = 1;

        [MinValue(1)]
        [SerializeField] private int _durationMinutes = 1440;

        internal QuestId QuestId => new(_questId);
        internal int RequiredCount => _requiredCount;
        internal int DurationMinutes => _durationMinutes;
    }

    [Serializable]
    internal sealed class PaymentRankUpRequirementEntry : RankUpRequirementEntry
    {
        [SerializeField] private PaymentEntry _payment = new();

        internal PaymentEntry Payment => _payment;
    }
}