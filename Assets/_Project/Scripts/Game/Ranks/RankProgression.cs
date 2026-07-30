using System;
using VContainer;

namespace LL.Game.Ranks
{
    internal sealed class RankProgression : IRankProgression
    {
        private readonly RankCatalog _catalog;

        [Inject]
        internal RankProgression(RankCatalog catalog)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        }

        public RankProgress GetProgress(RankId rankId, int experience)
        {
            var rank = _catalog.GetRank(rankId);

            if (experience < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(experience),
                    experience,
                    "Experience must not be negative.");
            }

            var hasNextRank = rank.Number < _catalog.Ranks.Count;
            var nextRank = hasNextRank
                ? _catalog.Ranks[rank.Number]
                : null;
            var requiredExperience = nextRank?.RequiredExperience ?? 0;

            if (hasNextRank && experience > requiredExperience)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(experience),
                    experience,
                    $"Experience at rank '{rank.Id}' must not exceed {requiredExperience}.");
            }

            if (hasNextRank is false && experience != 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(experience),
                    experience,
                    $"Experience at final rank '{rank.Id}' must be zero.");
            }

            return new RankProgress(rank, nextRank, experience);
        }

        public bool CanPromote(RankId rankId, int experience)
        {
            var progress = GetProgress(rankId, experience);

            return progress.HasNextRank &&
                   experience >= progress.RequiredExperience;
        }
    }
}