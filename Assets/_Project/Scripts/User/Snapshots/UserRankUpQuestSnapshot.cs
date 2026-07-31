using System;
using LL.Game.Identifiers;
using LL.Game.Quests;
using LL.Validation;

namespace LL.User.Snapshots
{
    internal sealed class UserRankUpQuestSnapshot
    {
        internal static UserRankUpQuestSnapshot Empty { get; } = new(default, 0L, false);

        internal QuestId QuestId { get; }
        internal long DeadlineUnixMilliseconds { get; }
        internal bool IsCompleted { get; }

        internal UserRankUpQuestSnapshot(
            QuestId questId,
            long deadlineUnixMilliseconds,
            bool isCompleted)
        {
            var hasQuest = ValidationChecks.IsNotEmpty(questId.Value);

            if (hasQuest is false)
            {
                if (deadlineUnixMilliseconds != 0L)
                    throw new ArgumentException(
                        "Rank-up quest without an ID must have a zero deadline.",
                        nameof(deadlineUnixMilliseconds));

                if (isCompleted)
                    throw new ArgumentException(
                        "Rank-up quest without an ID cannot be completed.",
                        nameof(isCompleted));
            }
            else if (deadlineUnixMilliseconds <= 0L)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(deadlineUnixMilliseconds),
                    deadlineUnixMilliseconds,
                    "Rank-up quest deadline must be greater than zero.");
            }

            if (hasQuest)
                IdentifierValidator.EnsureValid(questId, nameof(questId));

            QuestId = questId;
            DeadlineUnixMilliseconds = deadlineUnixMilliseconds;
            IsCompleted = isCompleted;
        }
    }
}