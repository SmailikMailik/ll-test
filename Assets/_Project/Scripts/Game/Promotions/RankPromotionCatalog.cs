using System;
using System.Collections.Generic;
using System.Linq;

namespace LL.Game.Promotions
{
    internal sealed class RankPromotionCatalog
    {
        private readonly IReadOnlyDictionary<int, RankPromotion> _promotions;

        internal RankPromotionCatalog(IEnumerable<RankPromotion> promotions)
        {
            var copy = promotions?.ToArray() ?? Array.Empty<RankPromotion>();

            if (copy.Any(promotion => promotion == null))
                throw new ArgumentException("Rank promotion entries must be non-null.", nameof(promotions));

            if (copy.Select(promotion => promotion.Rank).Distinct().Count() != copy.Length)
                throw new ArgumentException("Rank promotion entries must have unique ranks.", nameof(promotions));

            _promotions = copy.ToDictionary(promotion => promotion.Rank);
        }

        internal bool TryGetPromotion(int rank, out RankPromotion promotion)
        {
            return _promotions.TryGetValue(rank, out promotion);
        }
    }
}