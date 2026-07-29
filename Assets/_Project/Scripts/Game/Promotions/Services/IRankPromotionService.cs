using System;
using System.Collections.Generic;
using LL.Game.Items;

namespace LL.Game.Promotions.Services
{
    internal interface IRankPromotionService
    {
        bool TryGetPromotion(out RankPromotion promotion);

        void Promote(
            PromotionPaymentType paymentType,
            Action<IReadOnlyList<ItemAmount>> onSucceeded,
            Action onFailed);
    }
}