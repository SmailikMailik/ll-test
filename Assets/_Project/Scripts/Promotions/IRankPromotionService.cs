using System;
using System.Collections.Generic;
using LL.Game.Promotions;
using LL.Rewards.Models;

namespace LL.Promotions
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