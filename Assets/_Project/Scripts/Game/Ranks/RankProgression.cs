using System;
using System.Collections.Generic;
using VContainer;

namespace LL.Game.Ranks
{
    internal sealed class RankProgression : IRankProgression
    {
        private readonly IReadOnlyList<int> _experienceRequirements;

        [Inject]
        internal RankProgression(RankCatalog catalog)
        {
            _experienceRequirements = catalog?.ExperienceRequirements
                ?? throw new ArgumentNullException(nameof(catalog));
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
    }
}