using System;
using System.Collections.Generic;
using LL.Validation;

namespace LL.Game.Promotions
{
    internal static class RankPromotionCatalogValidator
    {
        private const string MissingEntryCode = "rank-promotion.entry.required";
        private const string InvalidRankCode = "rank-promotion.rank.positive";
        private const string DuplicateRankCode = "rank-promotion.rank.duplicate";

        internal static ValidationResult Validate<TPromotion>(
            IEnumerable<TPromotion> promotions,
            Func<TPromotion, int> getRank)
        {
            var result = new ValidationResult();
            Validate(promotions, getRank, new ValidationContext(result));
            return result;
        }

        internal static void Validate<TPromotion>(
            IEnumerable<TPromotion> promotions,
            Func<TPromotion, int> getRank,
            ValidationContext context)
        {
            if (getRank == null)
                throw new ArgumentNullException(nameof(getRank));

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var usedRanks = new HashSet<int>();
            var index = 0;

            if (promotions == null)
                return;

            foreach (var promotion in promotions)
            {
                var entryContext = context.At(index);

                if (ValidationRules.NotNull(promotion, entryContext, MissingEntryCode) is false)
                {
                    index++;
                    continue;
                }

                var rank = getRank(promotion);
                var rankContext = entryContext.At("Rank");

                if (ValidationRules.Positive(rank, rankContext, InvalidRankCode)
                    && usedRanks.Add(rank) is false)
                {
                    rankContext.Report(
                        ValidationSeverity.Error,
                        DuplicateRankCode,
                        $"Rank '{rank}' must be unique.");
                }

                index++;
            }
        }

        internal static bool IsValid<TPromotion>(
            IEnumerable<TPromotion> promotions,
            Func<TPromotion, int> getRank)
        {
            return Validate(promotions, getRank).IsValid;
        }

        internal static void EnsureValid<TPromotion>(
            IEnumerable<TPromotion> promotions,
            Func<TPromotion, int> getRank,
            string parameterName)
        {
            ValidationResultGuard.EnsureValid(
                Validate(promotions, getRank),
                parameterName);
        }
    }
}