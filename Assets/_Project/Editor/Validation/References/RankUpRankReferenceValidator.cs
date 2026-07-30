using System.Collections.Generic;
using LL.Game.Identifiers;
using LL.Game.RankUp.Configuration;
using LL.Game.Ranks;
using LL.Game.Ranks.Configuration;
using LL.Validation;
using LLEditor.Validation.Sources;
using UnityEditor;

namespace LLEditor.Validation.References
{
    internal sealed class RankUpRankReferenceValidator : IProjectDataReferenceValidation
    {
        private const string RankExistsCode = "rank-up.rank.exists";
        private const string FinalRankCode = "rank-up.rank.not-final";
        private const string RankUpRequiredCode = "rank-up.rank.required";

        public void Validate(
            ProjectDataSources sources,
            ValidationContext context)
        {
            var rankUps = sources.GetSingle<RankUpCatalogConfig>();
            var ranks = sources.GetSingle<RankCatalogConfig>();

            if (rankUps == null || ranks == null)
                return;

            ValidateReferences(
                rankUps.RankUps,
                ranks.Ranks,
                context.At(AssetDatabase.GetAssetPath(rankUps)));
        }

        private static void ValidateReferences(
            IReadOnlyList<RankUpEntry> rankUps,
            IReadOnlyList<RankEntry> ranks,
            ValidationContext context)
        {
            if (ranks == null || ranks.Count == 0)
                return;

            var rankIds = new HashSet<RankId>();

            for (var index = 0; index < ranks.Count; index++)
            {
                var rank = ranks[index];

                if (rank != null && IdentifierValidator.IsValid(rank.Id))
                    rankIds.Add(rank.Id);
            }

            var rankUpRankIds = new HashSet<RankId>();
            var rankUpCount = rankUps?.Count ?? 0;

            for (var index = 0; index < rankUpCount; index++)
            {
                var rankUp = rankUps[index];

                if (rankUp == null || IdentifierValidator.IsValid(rankUp.RankId) is false)
                    continue;

                rankUpRankIds.Add(rankUp.RankId);

                if (ValidationRules.ReferenceExists(
                        rankUp.RankId,
                        rankIds,
                        context.At(index).At(nameof(RankUpEntry.RankId)),
                        RankExistsCode) is false)
                {
                    continue;
                }

                var rankIndex = FindRankIndex(ranks, rankUp.RankId);

                if (rankIndex < 0 || rankIndex + 1 >= ranks.Count)
                {
                    context
                        .At(index)
                        .At(nameof(RankUpEntry.RankId))
                        .Report(
                            ValidationSeverity.Error,
                            FinalRankCode,
                            $"Final rank '{rankUp.RankId}' must not have a rank-up.");
                }
            }

            for (var index = 0; index + 1 < ranks.Count; index++)
            {
                var rank = ranks[index];

                if (rank == null ||
                    IdentifierValidator.IsValid(rank.Id) is false ||
                    rankUpRankIds.Contains(rank.Id))
                {
                    continue;
                }

                context.Report(
                    ValidationSeverity.Error,
                    RankUpRequiredCode,
                    $"Rank '{rank.Id}' must have a rank-up to the next rank.");
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