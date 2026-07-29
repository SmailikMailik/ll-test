using System;
using LL.Game.Promotions;

namespace LL.User.Snapshots
{
    internal sealed class UserPromotionOrderSnapshot
    {
        internal PromotionRequirementId RequirementId { get; }
        internal long DeadlineUnixMilliseconds { get; }
        internal bool IsCompleted { get; }

        internal UserPromotionOrderSnapshot(
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