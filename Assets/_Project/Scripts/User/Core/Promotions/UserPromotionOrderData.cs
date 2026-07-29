using System;
using LL.Game.Promotions;

namespace LL.User.Core.Promotions
{
    internal sealed class UserPromotionOrderData
    {
        internal PromotionRequirementId RequirementId { get; }
        internal long DeadlineUnixMilliseconds { get; }
        internal bool IsCompleted { get; }

        internal UserPromotionOrderData(
            PromotionRequirementId requirementId,
            long deadlineUnixMilliseconds,
            bool isCompleted)
        {
            RequirementId = requirementId;
            DeadlineUnixMilliseconds = requirementId.IsEmpty
                ? 0L
                : Math.Max(0L, deadlineUnixMilliseconds);
            IsCompleted = DeadlineUnixMilliseconds > 0L && isCompleted;
        }
    }
}