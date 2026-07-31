using System;
using LL.Validation;
using VContainer;

namespace LL.Game.Ranks
{
    internal sealed class RankProgression : IRankProgression
    {
        private const string ExperienceCode = "rank-progress.experience.non-negative";
        private const string ExperienceMaxCode = "rank-progress.experience.maximum";
        private const string FinalRankExperienceCode = "rank-progress.final-rank-experience.zero";

        private readonly RankCatalog _catalog;

        [Inject]
        internal RankProgression(RankCatalog catalog)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        }

        public RankProgress GetProgress(RankId rankId, int experience)
        {
            var rank = _catalog.GetRank(rankId);
            var rankIndex = rank.Number - 1;
            var hasNextRank = rankIndex + 1 < _catalog.Ranks.Count;
            var nextRank = hasNextRank
                ? _catalog.Ranks[rankIndex + 1]
                : null;
            var requiredExperience = nextRank?.RequiredExperience ?? 0;

            ValidationRunner.EnsureValid(
                context =>
                {
                    var experienceContext = context.At(nameof(experience));

                    if (ValidationRules.NonNegative(experience, experienceContext, ExperienceCode) is false)
                        return;

                    if (hasNextRank)
                    {
                        ValidationRules.LessThanOrEqual(
                            experience,
                            requiredExperience,
                            experienceContext,
                            ExperienceMaxCode);
                        return;
                    }

                    ValidationRules.Equal(
                        experience,
                        0,
                        experienceContext,
                        FinalRankExperienceCode);
                },
                nameof(experience));

            return new RankProgress(rank, nextRank, experience);
        }

        public bool CanRankUp(RankId rankId, int experience)
        {
            var progress = GetProgress(rankId, experience);

            return progress.HasNextRank &&
                   experience >= progress.RequiredExperience;
        }
    }
}