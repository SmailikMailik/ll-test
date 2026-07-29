using System;
using LL.Game.Identifiers;
using LL.Game.Promotions;

namespace LL.User.Snapshots
{
    internal sealed class UserPromotionOrderSnapshot
    {
        internal static UserPromotionOrderSnapshot Empty { get; } = new(default, 0L, false);

        internal PromotionRequirementId RequirementId { get; }
        internal long DeadlineUnixMilliseconds { get; }
        internal bool IsCompleted { get; }

        internal UserPromotionOrderSnapshot(
            PromotionRequirementId requirementId,
            long deadlineUnixMilliseconds,
            bool isCompleted)
        {
            if (string.IsNullOrWhiteSpace(requirementId.Value))
            {
                if (deadlineUnixMilliseconds != 0L)
                    throw new ArgumentException(
                        "Promotion order without a requirement must have a zero deadline.",
                        nameof(deadlineUnixMilliseconds));

                if (isCompleted)
                    throw new ArgumentException(
                        "Promotion order without a requirement cannot be completed.",
                        nameof(isCompleted));
            }
            else if (deadlineUnixMilliseconds <= 0L)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(deadlineUnixMilliseconds),
                    deadlineUnixMilliseconds,
                    "Promotion order deadline must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(requirementId.Value) is false)
                IdentifierValidator.EnsureValid(requirementId, nameof(requirementId));

            RequirementId = requirementId;
            DeadlineUnixMilliseconds = deadlineUnixMilliseconds;
            IsCompleted = isCompleted;
        }
    }
}