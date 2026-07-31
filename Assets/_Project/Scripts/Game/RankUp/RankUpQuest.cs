using System;
using LL.Game.Heroes;
using LL.Game.Identifiers;
using LL.Game.Payments;
using LL.Game.Quests;

namespace LL.Game.RankUp
{
    internal sealed class RankUpQuest
    {
        internal QuestId QuestId { get; }
        internal HeroId HeroId { get; }
        internal int RequiredAmount { get; }
        internal TimeSpan Duration { get; }
        internal Payment Payment { get; }

        internal RankUpQuest(
            QuestId questId,
            HeroId heroId,
            int requiredAmount,
            TimeSpan duration,
            Payment payment)
        {
            IdentifierValidator.EnsureValid(questId, nameof(questId));
            IdentifierValidator.EnsureValid(heroId, nameof(heroId));

            if (requiredAmount <= 0)
                throw new ArgumentOutOfRangeException(nameof(requiredAmount));

            if (duration <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(duration));

            QuestId = questId;
            HeroId = heroId;
            RequiredAmount = requiredAmount;
            Duration = duration;
            Payment = payment;
        }
    }
}