using System;
using System.Linq;
using LL.Game.Data.Validation;
using LL.Game.Ranks;
using LL.Infrastructure.Loading;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Game.Ranks.Configuration
{
    [CreateAssetMenu(fileName = nameof(RankCatalogConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class RankCatalogConfig : ScriptableObject, IDataLoader<RankCatalog>, IValidationSource
    {
        [ValidateInput(nameof(HasValidRequirements), "Rank catalog data is invalid.")]
        [InfoBox("Local experience required to reach each rank. Rank 1 always requires 0 XP.")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private RankExperienceRequirementEntry[] _rankRequirements;

        internal const string CreationPath = "LL/Game Data/Rank Catalog";

        private static readonly IDataValidator<RankExperienceRequirementEntry[]> _validator =
            new RankCatalogConfigValidator();

        internal RankExperienceRequirementEntry[] RankRequirements => _rankRequirements;

        public RankCatalog Load()
        {
            var result = ValidationRunner.Run(this);
            ValidationResultGuard.EnsureValid(result, nameof(_rankRequirements));

            return new RankCatalog(_rankRequirements.Select(requirement => requirement.RequiredExperience));
        }

        private static bool HasValidRequirements(RankExperienceRequirementEntry[] requirements)
        {
            return ValidationRunner.Run(requirements, _validator).IsValid;
        }

        void IValidationSource.Validate(ValidationContext context)
        {
            _validator.Validate(_rankRequirements, context);
        }
    }

    [Serializable]
    internal sealed class RankExperienceRequirementEntry
    {
        [MinValue(1)]
        [TableColumnWidth(40, Resizable = false)]
        [SerializeField] private int _rank = 1;

        [LabelText("Required Experience")]
        [SuffixLabel("XP", true)]
        [MinValue(0)]
        [SerializeField] private int _requiredExperience;

        internal int Rank => _rank;
        internal int RequiredExperience => _requiredExperience;
    }
}