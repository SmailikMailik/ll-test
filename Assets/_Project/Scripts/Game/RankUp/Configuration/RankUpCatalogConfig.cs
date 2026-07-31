using System;
using System.Collections.Generic;
using LL.Game.Payments.Configuration;
using LL.Game.Heroes;
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
        [SerializeField] private string _rankId;
        [SerializeField] private string _questId;
        [SerializeField] private string _heroId;

        [SerializeField, MinValue(1)] private int _requiredAmount = 1;
        [SerializeField, MinValue(1)] private int _durationMinutes = 1440;

        [SerializeField] private PaymentEntry _questPayment = new();
        [SerializeField] private PaymentEntry _instantPayment = new();
        [SerializeField] private string _rewardId;

        internal RankId RankId => new(_rankId);
        internal QuestId QuestId => new(_questId);
        internal HeroId HeroId => new(_heroId);
        internal int RequiredAmount => _requiredAmount;
        internal int DurationMinutes => _durationMinutes;
        internal PaymentEntry QuestPayment => _questPayment;
        internal PaymentEntry InstantPayment => _instantPayment;
        internal RewardId RewardId => new(_rewardId);
    }
}