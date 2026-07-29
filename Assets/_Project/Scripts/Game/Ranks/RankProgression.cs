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
            if (rank < 1 || rank > _experienceRequirements.Count)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(rank),
                    rank,
                    $"Rank must be between 1 and {_experienceRequirements.Count}.");
            }

            if (experience < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(experience),
                    experience,
                    "Experience must not be negative.");
            }

            var hasNextRank = rank < _experienceRequirements.Count;
            var requiredExperience = hasNextRank
                ? _experienceRequirements[rank]
                : 0;

            if (hasNextRank && experience > requiredExperience)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(experience),
                    experience,
                    $"Experience at rank {rank} must not exceed {requiredExperience}.");
            }

            if (hasNextRank is false && experience != 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(experience),
                    experience,
                    $"Experience at final rank {rank} must be zero.");
            }

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