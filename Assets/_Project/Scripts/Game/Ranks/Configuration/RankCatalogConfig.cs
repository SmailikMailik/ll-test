using System;
using System.Linq;
using LL.Game.Ranks;
using LL.Infrastructure.Loading;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Game.Ranks.Configuration
{
    [CreateAssetMenu(fileName = nameof(RankCatalogConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class RankCatalogConfig : ScriptableObject, IDataLoader<RankCatalog>
    {
        [InfoBox("Local experience required to reach each rank. Rank 1 always requires 0 XP.")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private RankExperienceRequirementEntry[] _rankRequirements;

        internal const string CreationPath = "LL/Game Data/Rank Catalog";

        public RankCatalog Load() => new
        (
            _rankRequirements?.Select(requirement => requirement?.RequiredExperience ?? 0)
        );

        private void OnValidate()
        {
            if (_rankRequirements == null || _rankRequirements.Length == 0)
            {
                _rankRequirements = new[] { new RankExperienceRequirementEntry(1, 0) };
                return;
            }

            for (var index = 0; index < _rankRequirements.Length; index++)
            {
                var rank = index + 1;
                var minimumExperience = index == 0
                    ? 0
                    : 1;

                var requirement = RankExperienceRequirementEntry.CreateOrNormalize(
                    _rankRequirements[index],
                    rank,
                    minimumExperience);

                _rankRequirements[index] = requirement;
            }
        }
    }

    [Serializable]
    internal sealed class RankExperienceRequirementEntry
    {
        [ReadOnly]
        [TableColumnWidth(40, Resizable = false)]
        [SerializeField] private int _rank;

        [LabelText("Required Experience")]
        [SuffixLabel("XP", true)]
        [MinValue(0)]
        [SerializeField] private int _requiredExperience;

        internal int RequiredExperience => _requiredExperience;

        internal RankExperienceRequirementEntry(int rank, int requiredExperience)
        {
            _rank = rank;
            _requiredExperience = requiredExperience;
        }

        internal static RankExperienceRequirementEntry CreateOrNormalize(
            RankExperienceRequirementEntry requirement,
            int rank,
            int minimumExperience)
        {
            requirement ??= new RankExperienceRequirementEntry(rank, minimumExperience);

            requirement._rank = rank;
            requirement._requiredExperience = rank == 1
                ? 0
                : Math.Max(requirement._requiredExperience, minimumExperience);

            return requirement;
        }
    }
}