using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Identifiers;
using LL.Game.Ranks;

namespace LL.Game.Promotions
{
    internal sealed class RankPromotionCatalog
    {
        internal IReadOnlyList<RankPromotion> Promotions { get; }

        private readonly IReadOnlyDictionary<RankId, RankPromotion> _promotionsByRankId;

        internal RankPromotionCatalog(IEnumerable<RankPromotion> promotions)
        {
            var entries = promotions?.ToArray() ?? Array.Empty<RankPromotion>();
            IdentifierCollectionValidator.EnsureValid(
                entries,
                promotion => promotion.RankId,
                nameof(promotions));

            Promotions = Array.AsReadOnly(entries);
            _promotionsByRankId = entries.ToDictionary(promotion => promotion.RankId);
        }

        internal bool TryGetPromotion(RankId rankId, out RankPromotion promotion) =>
            _promotionsByRankId.TryGetValue(rankId, out promotion);
    }
}