using System;
using LL.Game.Promotions;
using R3;

namespace LL.User.State.Promotions
{
    internal interface IUserPromotionOrder
    {
        PromotionRequirementId RequirementId { get; }
        long DeadlineUnixMilliseconds { get; }
        bool IsActive { get; }
        bool IsCompleted { get; }
        Observable<Unit> Changed { get; }

        TimeSpan GetRemainingTime();
        bool TryStart(PromotionRequirementId requirementId, TimeSpan duration);
        bool TryComplete();
        bool TryExpire();
        void ClearOrder();
    }
}