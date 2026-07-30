using System;
using LL.Game.Identifiers;
using LL.Game.Quests;

namespace LL.User.Snapshots
{
    internal sealed class UserPromotionQuestSnapshot
    {
        internal static UserPromotionQuestSnapshot Empty { get; } = new(default, 0L, false);

        internal QuestId QuestId { get; }
        internal long DeadlineUnixMilliseconds { get; }
        internal bool IsCompleted { get; }

        internal UserPromotionQuestSnapshot(
            QuestId questId,
            long deadlineUnixMilliseconds,
            bool isCompleted)
        {
            if (string.IsNullOrWhiteSpace(questId.Value))
            {
                if (deadlineUnixMilliseconds != 0L)
                    throw new ArgumentException(
                        "Promotion quest without an ID must have a zero deadline.",
                        nameof(deadlineUnixMilliseconds));

                if (isCompleted)
                    throw new ArgumentException(
                        "Promotion quest without an ID cannot be completed.",
                        nameof(isCompleted));
            }
            else if (deadlineUnixMilliseconds <= 0L)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(deadlineUnixMilliseconds),
                    deadlineUnixMilliseconds,
                    "Promotion quest deadline must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(questId.Value) is false)
                IdentifierValidator.EnsureValid(questId, nameof(questId));

            QuestId = questId;
            DeadlineUnixMilliseconds = deadlineUnixMilliseconds;
            IsCompleted = isCompleted;
        }
    }
}