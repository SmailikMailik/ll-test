using System;
using LL.Game.Identifiers;
using LL.Game.Quests;
using LL.Validation;

namespace LL.Game.RankUp
{
    internal sealed class QuestRankUpRequirementDefinition : RankUpRequirementDefinition
    {
        internal QuestId QuestId { get; }
        internal int RequiredCount { get; }
        internal TimeSpan Duration { get; }

        internal QuestRankUpRequirementDefinition(
            RankUpRequirementId id,
            QuestId questId,
            int requiredCount,
            TimeSpan duration)
            : base(id)
        {
            IdentifierValidator.EnsureValid(questId, nameof(questId));

            if (ValidationChecks.IsNonPositive(requiredCount))
                throw new ArgumentOutOfRangeException(nameof(requiredCount));

            if (duration <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(duration));

            QuestId = questId;
            RequiredCount = requiredCount;
            Duration = duration;
        }
    }
}