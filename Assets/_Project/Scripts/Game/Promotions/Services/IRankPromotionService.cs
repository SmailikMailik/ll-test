using System.Collections.Generic;
using LL.Game.Items;
using LL.Game.Payments;

namespace LL.Game.Promotions.Services
{
    internal interface IRankPromotionService
    {
        bool TryGetPromotion(out RankPromotion promotion);
        bool CanPromote(Payment payment);
        bool TryPromote(Payment payment, out IReadOnlyList<ItemAmount> rewardItems);
    }
}