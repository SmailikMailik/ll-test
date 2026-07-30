using System.Collections.Generic;
using LL.Game.Identifiers;
using LL.Game.Promotions.Configuration;
using LL.Game.Ranks;
using LL.Game.Ranks.Configuration;
using LL.Validation;
using LLEditor.Validation.Sources;
using UnityEditor;

namespace LLEditor.Validation.References
{
    internal sealed class RankPromotionRankReferenceValidator : IProjectDataReferenceValidation
    {
        private const string RankExistsCode = "rank-promotion.rank.exists";
        private const string FinalRankCode = "rank-promotion.rank.not-final";
        private const string PromotionRequiredCode = "rank-promotion.rank.required";

        public void Validate(
            ProjectDataSources sources,
            ValidationContext context)
        {
            var promotions = sources.GetSingle<RankPromotionCatalogConfig>();
            var ranks = sources.GetSingle<RankCatalogConfig>();

            if (promotions == null || ranks == null)
                return;

            ValidateReferences(
                promotions.Promotions,
                ranks.Ranks,
                context.At(AssetDatabase.GetAssetPath(promotions)));
        }

        private static void ValidateReferences(
            IReadOnlyList<RankPromotionEntry> promotions,
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

            var promotionRankIds = new HashSet<RankId>();
            var promotionCount = promotions?.Count ?? 0;

            for (var index = 0; index < promotionCount; index++)
            {
                var promotion = promotions[index];

                if (promotion == null || IdentifierValidator.IsValid(promotion.RankId) is false)
                    continue;

                promotionRankIds.Add(promotion.RankId);

                if (ValidationRules.ReferenceExists(
                        promotion.RankId,
                        rankIds,
                        context.At(index).At(nameof(RankPromotionEntry.RankId)),
                        RankExistsCode) is false)
                {
                    continue;
                }

                var rankIndex = FindRankIndex(ranks, promotion.RankId);

                if (rankIndex < 0 || rankIndex + 1 >= ranks.Count)
                {
                    context
                        .At(index)
                        .At(nameof(RankPromotionEntry.RankId))
                        .Report(
                            ValidationSeverity.Error,
                            FinalRankCode,
                            $"Final rank '{promotion.RankId}' must not have a promotion.");
                }
            }

            for (var index = 0; index + 1 < ranks.Count; index++)
            {
                var rank = ranks[index];

                if (rank == null ||
                    IdentifierValidator.IsValid(rank.Id) is false ||
                    promotionRankIds.Contains(rank.Id))
                {
                    continue;
                }

                context.Report(
                    ValidationSeverity.Error,
                    PromotionRequiredCode,
                    $"Rank '{rank.Id}' must have a promotion to the next rank.");
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