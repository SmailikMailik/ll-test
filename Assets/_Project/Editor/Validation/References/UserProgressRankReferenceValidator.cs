using System.Collections.Generic;
using LL.Game.Identifiers;
using LL.Game.Ranks;
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
                ranks.Ranks,
                context
                    .At(AssetDatabase.GetAssetPath(userDefaults))
                    .At(nameof(UserDefaultsConfig.Progress)));
        }

        private static void ValidateReferences(
            UserProgressDefaults progress,
            IReadOnlyList<RankEntry> ranks,
            ValidationContext context)
        {
            if (progress == null ||
                IdentifierValidator.IsValid(progress.RankId) is false ||
                ranks == null ||
                ranks.Count == 0)
            {
                return;
            }

            var rankIds = new HashSet<RankId>();

            for (var index = 0; index < ranks.Count; index++)
            {
                var rank = ranks[index];

                if (rank != null && IdentifierValidator.IsValid(rank.Id))
                    rankIds.Add(rank.Id);
            }

            if (ValidationRules.ReferenceExists(
                    progress.RankId,
                    rankIds,
                    context.At(nameof(UserProgressDefaults.RankId)),
                    RankExistsCode) is false)
            {
                return;
            }

            if (progress.Experience < 0)
                return;

            var rankIndex = FindRankIndex(ranks, progress.RankId);
            var nextRank = rankIndex >= 0 && rankIndex + 1 < ranks.Count
                ? ranks[rankIndex + 1]
                : null;

            if (nextRank == null)
            {
                if (progress.Experience != 0)
                {
                    context
                        .At(nameof(UserProgressDefaults.Experience))
                        .Report(
                            ValidationSeverity.Error,
                            FinalRankExperienceCode,
                            $"Experience at final rank '{progress.RankId}' must be zero.");
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
                        $"Experience at rank '{progress.RankId}' must not exceed " +
                        $"{nextRank.RequiredExperience}.");
            }
        }

        private static int FindRankIndex(
            IReadOnlyList<RankEntry> ranks,
            RankId rankId)
        {
            for (var index = 0; index < ranks.Count; index++)
            {
                if (ranks[index]?.Id.Equals(rankId) == true)
                    return index;
            }

            return -1;
        }
    }
}