using System;
using System.Collections.Generic;
using LL.Validation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Game.Ranks.Configuration
{
    [CreateAssetMenu(fileName = nameof(RankCatalogConfig), menuName = CreationPath)]
    internal sealed class RankCatalogConfig : ScriptableObject, IValidationSource
    {
        [ValidateInput(nameof(HasValidRanks), "Rank catalog data is invalid.")]
        [SerializeField] private RankEntry[] _ranks;

        internal const string CreationPath = "LL/Game Data/Rank Catalog";

        private static readonly IDataValidator<RankEntry[]> _validator = new RankCatalogConfigValidator();

        internal IReadOnlyList<RankEntry> Ranks => _ranks;

        private static bool HasValidRanks(RankEntry[] ranks)
        {
            return ValidationRunner.IsValid(ranks, _validator);
        }

        void IValidationSource.Validate(ValidationContext context)
        {
            _validator.Validate(_ranks, context);
        }
    }

    [Serializable]
    internal sealed class RankEntry
    {
        [SerializeField] private string _id;
        [SerializeField, MinValue(0)] private int _requiredExperience;

        internal RankId Id => new(_id);
        internal int RequiredExperience => _requiredExperience;
    }
}