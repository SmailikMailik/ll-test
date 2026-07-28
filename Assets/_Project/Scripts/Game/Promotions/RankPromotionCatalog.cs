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
            RankPromotionCatalogValidator.EnsureValidRanks(
                copy,
                promotion => promotion.Rank,
                nameof(promotions));

            _promotions = copy.ToDictionary(promotion => promotion.Rank);
        }

        internal bool TryGetPromotion(int rank, out RankPromotion promotion)
        {
            return _promotions.TryGetValue(rank, out promotion);
        }
    }
}