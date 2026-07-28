using System;
using LL.Game.Promotions;

namespace LL.Promotions
{
    internal interface IRankPromotionService
    {
        bool TryGetPromotion(out RankPromotion promotion);

        void Purchase(
            PromotionPaymentType paymentType,
            bool requirementCompleted,
            Action onSucceeded,
            Action onFailed);
    }
}