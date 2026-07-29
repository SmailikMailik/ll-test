using System.Collections.Generic;
using System.Linq;
using LL.Game.Data.Validation;
using LL.Game.Ranks.Configuration;

namespace LL.User.Configuration
{
    internal sealed class UserProgressRankReferenceValidator
    {
        private const string RankExistsCode = "user-defaults.progress.rank.exists";
        private const string ExperienceMaximumCode = "user-defaults.progress.experience.maximum";
        private const string FinalRankExperienceCode = "user-defaults.progress.final-rank-experience.zero";

        internal void Validate(
            UserProgressDefaults progress,
            IReadOnlyList<RankExperienceRequirementEntry> rankRequirements,
            ValidationContext context)
        {
            if (progress == null || rankRequirements == null || rankRequirements.Count == 0)
                return;

            var ranks = new HashSet<int>(
                rankRequirements
                    .Where(requirement => requirement != null)
                    .Select(requirement => requirement.Rank));

            if (ValidationRules.ReferenceExists(
                    progress.Rank,
                    ranks,
                    context.At("Rank"),
                    RankExistsCode) is false)
            {
                return;
            }

            var nextRank = rankRequirements.FirstOrDefault(
                requirement => requirement != null && requirement.Rank == progress.Rank + 1);

            if (nextRank == null)
            {
                if (progress.Experience != 0)
                {
                    context
                        .At("Experience")
                        .Report(
                            ValidationSeverity.Error,
                            FinalRankExperienceCode,
                            $"Experience at final rank '{progress.Rank}' must be zero.");
                }

                return;
            }

            if (progress.Experience > nextRank.RequiredExperience)
            {
                context
                    .At("Experience")
                    .Report(
                        ValidationSeverity.Error,
                        ExperienceMaximumCode,
                        $"Experience at rank '{progress.Rank}' must not exceed " +
                        $"{nextRank.RequiredExperience}.");
            }
        }
    }
}