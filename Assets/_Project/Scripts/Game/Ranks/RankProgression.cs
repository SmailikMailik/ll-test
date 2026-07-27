using System;
using System.Collections.Generic;
using LL.Loading;
using VContainer;

namespace LL.Game.Ranks
{
    internal sealed class RankProgression : IRankProgression
    {
        private readonly IReadOnlyList<int> _experienceRequirements;

        [Inject]
        internal RankProgression(IDataLoader<RankCatalog> loader)
        {
            if (loader is null)
                throw new ArgumentNullException(nameof(loader));

            _experienceRequirements = Normalize(loader.Load()?.ExperienceRequirements);
        }

        public RankProgress GetProgress(int rank, int experience)
        {
            rank = Math.Clamp(rank, 1, _experienceRequirements.Count);
            experience = Math.Max(0, experience);

            var hasNextRank = rank < _experienceRequirements.Count;
            var requiredExperience = hasNextRank
                ? _experienceRequirements[rank]
                : 0;

            return new RankProgress(
                rank,
                experience,
                requiredExperience,
                hasNextRank);
        }

        public bool CanPromote(int rank, int experience)
        {
            var progress = GetProgress(rank, experience);

            return progress.HasNextRank &&
                   experience >= progress.RequiredExperience;
        }

        private static IReadOnlyList<int> Normalize(IReadOnlyList<int> requirements)
        {
            if (requirements == null || requirements.Count == 0)
                return new[] { 0 };

            var normalized = new int[requirements.Count];

            for (var index = 1; index < requirements.Count; index++)
                normalized[index] = Math.Max(1, requirements[index]);

            return normalized;
        }
    }
}