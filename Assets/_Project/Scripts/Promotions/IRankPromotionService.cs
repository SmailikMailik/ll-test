using System;
using System.Collections.Generic;
using LL.Game.Promotions;
using LL.Rewards.Models;

namespace LL.Promotions
{
    internal interface IRankPromotionService
    {
        bool TryGetPromotion(out RankPromotion promotion);

        void Purchase(
            PromotionPaymentType paymentType,
            bool requirementCompleted,
            Action<IReadOnlyList<IReward>> onSucceeded,
            Action onFailed);
    }
}