using System;

namespace LL.Game.Data.Persistence
{
    internal sealed class GameDataDocument
    {
        internal const int CurrentVersion = 1;

        public int Version { get; }
        public RankData[] Ranks { get; }
        public CardData[] Cards { get; }
        public QuestData[] Quests { get; }
        public RankPromotionData[] RankPromotions { get; }
        public RewardData[] Rewards { get; }

        public GameDataDocument(
            int version,
            RankData[] ranks,
            CardData[] cards,
            QuestData[] quests,
            RankPromotionData[] rankPromotions,
            RewardData[] rewards)
        {
            Version = version;
            Ranks = ranks ?? Array.Empty<RankData>();
            Cards = cards ?? Array.Empty<CardData>();
            Quests = quests ?? Array.Empty<QuestData>();
            RankPromotions = rankPromotions ?? Array.Empty<RankPromotionData>();
            Rewards = rewards ?? Array.Empty<RewardData>();
        }
    }

    internal sealed class RankData
    {
        public string Id { get; }
        public int RequiredExperience { get; }

        public RankData(string id, int requiredExperience)
        {
            Id = id;
            RequiredExperience = requiredExperience;
        }
    }

    internal sealed class CardData
    {
        public string Id { get; }
        public int ExperienceAmount { get; }

        public CardData(string id, int experienceAmount)
        {
            Id = id;
            ExperienceAmount = experienceAmount;
        }
    }

    internal sealed class QuestData
    {
        public string Id { get; }
        public string TitleLocalizationKey { get; }
        public string DescriptionLocalizationKey { get; }

        public QuestData(
            string id,
            string titleLocalizationKey,
            string descriptionLocalizationKey)
        {
            Id = id;
            TitleLocalizationKey = titleLocalizationKey;
            DescriptionLocalizationKey = descriptionLocalizationKey;
        }
    }

    internal sealed class RankPromotionData
    {
        public string RankId { get; }
        public string QuestId { get; }
        public string HeroLocalizationKey { get; }
        public int RequiredAmount { get; }
        public int DurationMinutes { get; }
        public PaymentData QuestPayment { get; }
        public PaymentData InstantPayment { get; }
        public string RewardId { get; }

        public RankPromotionData(
            string rankId,
            string questId,
            string heroLocalizationKey,
            int requiredAmount,
            int durationMinutes,
            PaymentData questPayment,
            PaymentData instantPayment,
            string rewardId)
        {
            RankId = rankId;
            QuestId = questId;
            HeroLocalizationKey = heroLocalizationKey;
            RequiredAmount = requiredAmount;
            DurationMinutes = durationMinutes;
            QuestPayment = questPayment ?? throw new ArgumentNullException(nameof(questPayment));
            InstantPayment = instantPayment ?? throw new ArgumentNullException(nameof(instantPayment));
            RewardId = rewardId;
        }
    }

    internal sealed class PaymentData
    {
        public string ItemId { get; }
        public int Amount { get; }

        public PaymentData(string itemId, int amount)
        {
            ItemId = itemId;
            Amount = amount;
        }
    }

    internal sealed class RewardData
    {
        public string Id { get; }
        public ItemAmountData[] Items { get; }

        public RewardData(string id, ItemAmountData[] items)
        {
            Id = id;
            Items = items ?? Array.Empty<ItemAmountData>();
        }
    }

    internal sealed class ItemAmountData
    {
        public string Id { get; }
        public int Amount { get; }

        public ItemAmountData(string id, int amount)
        {
            Id = id;
            Amount = amount;
        }
    }
}