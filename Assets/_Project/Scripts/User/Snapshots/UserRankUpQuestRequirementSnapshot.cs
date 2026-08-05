using System;
using LL.Game.Identifiers;
using LL.Game.RankUp;
using LL.Validation;

namespace LL.User.Snapshots
{
    internal sealed class UserRankUpQuestRequirementSnapshot
    {
        internal RankUpRequirementId RequirementId { get; }
        internal int CurrentCount { get; }
        internal long DeadlineUnixMilliseconds { get; }

        internal UserRankUpQuestRequirementSnapshot(
            RankUpRequirementId requirementId,
            int currentCount,
            long deadlineUnixMilliseconds)
        {
            IdentifierValidator.EnsureValid(requirementId, nameof(requirementId));

            if (ValidationChecks.IsNegative(currentCount))
                throw new ArgumentOutOfRangeException(nameof(currentCount));

            if (deadlineUnixMilliseconds <= 0L)
                throw new ArgumentOutOfRangeException(nameof(deadlineUnixMilliseconds));

            RequirementId = requirementId;
            CurrentCount = currentCount;
            DeadlineUnixMilliseconds = deadlineUnixMilliseconds;
        }
    }
}