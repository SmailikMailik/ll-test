using System;
using System.Collections.Generic;

namespace LL.Game.Promotions
{
    internal static class RankPromotionCatalogValidator
    {
        internal static bool HasValidRanks<TPromotion>(
            IEnumerable<TPromotion> promotions,
            Func<TPromotion, int> rankSelector)
            where TPromotion : class
        {
            if (rankSelector == null)
                throw new ArgumentNullException(nameof(rankSelector));

            if (promotions == null)
                return true;

            var ranks = new HashSet<int>();

            foreach (var promotion in promotions)
            {
                if (promotion == null)
                    return false;

                var rank = rankSelector(promotion);

                if (rank < 1 || ranks.Add(rank) is false)
                    return false;
            }

            return true;
        }

        internal static void EnsureValidRanks<TPromotion>(
            IEnumerable<TPromotion> promotions,
            Func<TPromotion, int> rankSelector,
            string parameterName)
            where TPromotion : class
        {
            if (HasValidRanks(promotions, rankSelector))
                return;

            throw new ArgumentException(
                "Rank promotion entries must be non-null and have unique positive ranks.",
                parameterName);
        }
    }
}