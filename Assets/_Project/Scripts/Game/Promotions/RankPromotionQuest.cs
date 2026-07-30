using System;
using LL.Game.Payments;
using LL.Game.Quests;

namespace LL.Game.Promotions
{
    internal sealed class RankPromotionQuest
    {
        internal QuestId QuestId { get; }
        internal string HeroLocalizationKey { get; }
        internal int RequiredAmount { get; }
        internal TimeSpan Duration { get; }
        internal Payment Payment { get; }

        internal RankPromotionQuest(
            QuestId questId,
            string heroLocalizationKey,
            int requiredAmount,
            TimeSpan duration,
            Payment payment)
        {
            if (string.IsNullOrWhiteSpace(questId.Value))
                throw new ArgumentException("Promotion quest ID must be non-empty.", nameof(questId));

            if (string.IsNullOrWhiteSpace(heroLocalizationKey))
                throw new ArgumentException("Promotion quest hero localization key must be non-empty.");

            if (requiredAmount <= 0)
                throw new ArgumentOutOfRangeException(nameof(requiredAmount));

            if (duration <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(duration));

            QuestId = questId;
            HeroLocalizationKey = heroLocalizationKey;
            RequiredAmount = requiredAmount;
            Duration = duration;
            Payment = payment;
        }
    }
}