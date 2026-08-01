using System;
using LL.Game.Heroes;
using LL.Game.Identifiers;
using LL.Game.Payments;
using LL.Game.Quests;
using LL.Validation;

namespace LL.Game.RankUp
{
    internal sealed class RankUpQuest
    {
        internal QuestId QuestId { get; }
        internal HeroId HeroId { get; }
        internal int RequiredCount { get; }
        internal TimeSpan Duration { get; }
        internal Payment Payment { get; }

        internal RankUpQuest(
            QuestId questId,
            HeroId heroId,
            int requiredCount,
            TimeSpan duration,
            Payment payment)
        {
            IdentifierValidator.EnsureValid(questId, nameof(questId));
            IdentifierValidator.EnsureValid(heroId, nameof(heroId));

            if (ValidationChecks.IsNonPositive(requiredCount))
                throw new ArgumentOutOfRangeException(nameof(requiredCount));

            if (duration <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(duration));

            QuestId = questId;
            HeroId = heroId;
            RequiredCount = requiredCount;
            Duration = duration;
            Payment = payment;
        }
    }
}