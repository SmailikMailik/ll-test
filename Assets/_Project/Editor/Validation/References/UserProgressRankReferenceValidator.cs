using System.Collections.Generic;
using System.Linq;
using LL.Game.Ranks.Configuration;
using LL.User.Configuration;
using LL.Validation;
using LLEditor.Validation.Sources;
using UnityEditor;

namespace LLEditor.Validation.References
{
    internal sealed class UserProgressRankReferenceValidator : IProjectDataReferenceValidation
    {
        private const string RankExistsCode = "user-defaults.progress.rank.exists";
        private const string ExperienceMaximumCode = "user-defaults.progress.experience.maximum";
        private const string FinalRankExperienceCode = "user-defaults.progress.final-rank-experience.zero";

        public void Validate(
            ProjectDataSources sources,
            ValidationContext context)
        {
            var ranks = sources.GetSingle<RankCatalogConfig>();
            var userDefaults = sources.GetSingle<UserDefaultsConfig>();

            if (ranks == null || userDefaults == null)
                return;

            ValidateReferences(
                userDefaults.Progress,
                ranks.RankRequirements,
                context
                    .At(AssetDatabase.GetAssetPath(userDefaults))
                    .At(nameof(UserDefaultsConfig.Progress)));
        }

        private static void ValidateReferences(
            UserProgressDefaults progress,
            IReadOnlyList<RankExperienceRequirementEntry> rankRequirements,
            ValidationContext context)
        {
            if (progress == null ||
                progress.Rank <= 0 ||
                rankRequirements == null ||
                rankRequirements.Count == 0)
            {
                return;
            }

            var ranks = new HashSet<int>(
                rankRequirements
                    .Where(requirement => requirement != null)
                    .Select(requirement => requirement.Rank));

            if (ValidationRules.ReferenceExists(
                    progress.Rank,
                    ranks,
                    context.At(nameof(UserProgressDefaults.Rank)),
                    RankExistsCode) is false)
            {
                return;
            }

            if (progress.Experience < 0)
                return;

            var nextRank = rankRequirements.FirstOrDefault(
                requirement => requirement != null && requirement.Rank == progress.Rank + 1);

            if (nextRank == null)
            {
                if (progress.Experience != 0)
                {
                    context
                        .At(nameof(UserProgressDefaults.Experience))
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
                    .At(nameof(UserProgressDefaults.Experience))
                    .Report(
                        ValidationSeverity.Error,
                        ExperienceMaximumCode,
                        $"Experience at rank '{progress.Rank}' must not exceed " +
                        $"{nextRank.RequiredExperience}.");
            }
        }
    }
}