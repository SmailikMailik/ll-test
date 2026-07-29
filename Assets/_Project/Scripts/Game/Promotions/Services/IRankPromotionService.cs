using System;
using System.Collections.Generic;
using LL.Game.Rewards;

namespace LL.Game.Promotions.Services
{
    internal interface IRankPromotionService
    {
        bool TryGetPromotion(out RankPromotion promotion);

        void Promote(
            PromotionPaymentType paymentType,
            Action<IReadOnlyList<IReward>> onSucceeded,
            Action onFailed);
    }
}