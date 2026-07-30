using System;
using System.Collections.Generic;

namespace LL.Game.Data
{
    internal sealed class GameDataDeclaration
    {
        internal IReadOnlyList<RankDeclaration> Ranks { get; }
        internal IReadOnlyList<CardDeclaration> Cards { get; }
        internal IReadOnlyList<QuestDeclaration> Quests { get; }
        internal IReadOnlyList<RankPromotionDeclaration> RankPromotions { get; }
        internal IReadOnlyList<RewardDeclaration> Rewards { get; }

        internal GameDataDeclaration(
            IEnumerable<RankDeclaration> ranks,
            IEnumerable<CardDeclaration> cards,
            IEnumerable<QuestDeclaration> quests,
            IEnumerable<RankPromotionDeclaration> rankPromotions,
            IEnumerable<RewardDeclaration> rewards)
        {
            Ranks = Copy(ranks);
            Cards = Copy(cards);
            Quests = Copy(quests);
            RankPromotions = Copy(rankPromotions);
            Rewards = Copy(rewards);
        }

        private static IReadOnlyList<T> Copy<T>(IEnumerable<T> entries)
        {
            return Array.AsReadOnly(entries == null ? Array.Empty<T>() : new List<T>(entries).ToArray());
        }
    }

    internal sealed class RankDeclaration
    {
        internal string Id { get; }
        internal int RequiredExperience { get; }

        internal RankDeclaration(string id, int requiredExperience)
        {
            Id = id;
            RequiredExperience = requiredExperience;
        }
    }

    internal sealed class CardDeclaration
    {
        internal string Id { get; }
        internal int ExperienceAmount { get; }

        internal CardDeclaration(string id, int experienceAmount)
        {
            Id = id;
            ExperienceAmount = experienceAmount;
        }
    }

    internal sealed class QuestDeclaration
    {
        internal string Id { get; }
        internal string TitleLocalizationKey { get; }
        internal string DescriptionLocalizationKey { get; }

        internal QuestDeclaration(string id, string titleLocalizationKey, string descriptionLocalizationKey)
        {
            Id = id;
            TitleLocalizationKey = titleLocalizationKey;
            DescriptionLocalizationKey = descriptionLocalizationKey;
        }
    }

    internal sealed class RankPromotionDeclaration
    {
        internal string RankId { get; }
        internal string QuestId { get; }
        internal string HeroLocalizationKey { get; }
        internal int RequiredAmount { get; }
        internal int DurationMinutes { get; }
        internal PaymentDeclaration QuestPayment { get; }
        internal PaymentDeclaration InstantPayment { get; }
        internal string RewardId { get; }

        internal RankPromotionDeclaration(
            string rankId,
            string questId,
            string heroLocalizationKey,
            int requiredAmount,
            int durationMinutes,
            PaymentDeclaration questPayment,
            PaymentDeclaration instantPayment,
            string rewardId)
        {
            RankId = rankId;
            QuestId = questId;
            HeroLocalizationKey = heroLocalizationKey;
            RequiredAmount = requiredAmount;
            DurationMinutes = durationMinutes;
            QuestPayment = questPayment;
            InstantPayment = instantPayment;
            RewardId = rewardId;
        }
    }

    internal sealed class PaymentDeclaration
    {
        internal string ItemId { get; }
        internal int Amount { get; }

        internal PaymentDeclaration(string itemId, int amount)
        {
            ItemId = itemId;
            Amount = amount;
        }
    }

    internal sealed class RewardDeclaration
    {
        internal string Id { get; }
        internal IReadOnlyList<ItemAmountDeclaration> Items { get; }

        internal RewardDeclaration(string id, IEnumerable<ItemAmountDeclaration> items)
        {
            Id = id;
            var copy = items == null
                ? Array.Empty<ItemAmountDeclaration>()
                : new List<ItemAmountDeclaration>(items).ToArray();

            Items = Array.AsReadOnly(copy);
        }
    }

    internal sealed class ItemAmountDeclaration
    {
        internal string Id { get; }
        internal int Amount { get; }

        internal ItemAmountDeclaration(string id, int amount)
        {
            Id = id;
            Amount = amount;
        }
    }
}