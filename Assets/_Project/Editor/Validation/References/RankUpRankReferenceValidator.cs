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
            if (ranks is null || ranks.Count == 0)
                return;

            var rankIds = new HashSet<RankId>();
            var orderedRankIds = new List<RankId>(ranks.Count);

            for (var index = 0; index < ranks.Count; index++)
            {
                var rank = ranks[index];

                if (rank is not null && IdentifierValidator.IsValid(rank.Id))
                {
                    rankIds.Add(rank.Id);
                    orderedRankIds.Add(rank.Id);
                }
            }

            var rankUpRankIds = new HashSet<RankId>();
            var rankUpCount = rankUps?.Count ?? 0;

            for (var index = 0; index < rankUpCount; index++)
            {
                var rankUp = rankUps[index];

                if (rankUp is null || IdentifierValidator.IsValid(rankUp.RankId) is false)
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
            }

            if (orderedRankIds.Count != ranks.Count || rankIds.Count != ranks.Count)
                return;

            var finalRankId = orderedRankIds[orderedRankIds.Count - 1];

            for (var index = 0; index < rankUpCount; index++)
            {
                var rankUp = rankUps[index];

                if (rankUp is null || IdentifierValidator.IsValid(rankUp.RankId) is false)
                    continue;

                ValidationRules.NotEqual(
                    rankUp.RankId,
                    finalRankId,
                    context.At(index).At(nameof(RankUpEntry.RankId)),
                    FinalRankCode);
            }

            for (var index = 0; index + 1 < orderedRankIds.Count; index++)
            {
                ValidationRules.ReferenceExists(
                    orderedRankIds[index],
                    rankUpRankIds,
                    context,
                    RankUpRequiredCode);
            }
        }
    }
}