using System.Collections.Generic;
using LL.Game.Promotions.Configuration;
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
                ranks.RankRequirements,
                context.At(AssetDatabase.GetAssetPath(promotions)));
        }

        private static void ValidateReferences(
            IReadOnlyList<RankPromotionEntry> promotions,
            IReadOnlyList<RankExperienceRequirementEntry> rankRequirements,
            ValidationContext context)
        {
            if (rankRequirements == null || rankRequirements.Count == 0)
                return;

            var ranks = new HashSet<int>();

            foreach (var requirement in rankRequirements)
            {
                if (requirement != null)
                    ranks.Add(requirement.Rank);
            }

            var promotionRanks = new HashSet<int>();
            var promotionCount = promotions?.Count ?? 0;

            for (var index = 0; index < promotionCount; index++)
            {
                var promotion = promotions[index];

                if (promotion == null || promotion.Rank <= 0)
                    continue;

                promotionRanks.Add(promotion.Rank);

                if (ValidationRules.ReferenceExists(
                        promotion.Rank,
                        ranks,
                        context.At(index).At("Rank"),
                        RankExistsCode) is false)
                {
                    continue;
                }

                if (ranks.Contains(promotion.Rank + 1) is false)
                {
                    context
                        .At(index)
                        .At("Rank")
                        .Report(
                            ValidationSeverity.Error,
                            FinalRankCode,
                            $"Final rank '{promotion.Rank}' must not have a promotion.");
                }
            }

            foreach (var requirement in rankRequirements)
            {
                if (requirement == null ||
                    ranks.Contains(requirement.Rank + 1) is false ||
                    promotionRanks.Contains(requirement.Rank))
                {
                    continue;
                }

                context.Report(
                    ValidationSeverity.Error,
                    PromotionRequiredCode,
                    $"Rank '{requirement.Rank}' must have a promotion to the next rank.");
            }
        }
    }
}