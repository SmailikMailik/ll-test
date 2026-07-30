using System;

namespace LL.Game.Data.Persistence
{
    internal sealed class GameDataDocument
    {
        internal const int CurrentVersion = 1;

        public int Version { get; }
        public RankDocumentEntry[] Ranks { get; }
        public CardDocumentEntry[] Cards { get; }
        public QuestDocumentEntry[] Quests { get; }
        public RankPromotionDocumentEntry[] RankPromotions { get; }
        public RewardDocumentEntry[] Rewards { get; }

        public GameDataDocument(
            int version,
            RankDocumentEntry[] ranks,
            CardDocumentEntry[] cards,
            QuestDocumentEntry[] quests,
            RankPromotionDocumentEntry[] rankPromotions,
            RewardDocumentEntry[] rewards)
        {
            Version = version;
            Ranks = ranks ?? Array.Empty<RankDocumentEntry>();
            Cards = cards ?? Array.Empty<CardDocumentEntry>();
            Quests = quests ?? Array.Empty<QuestDocumentEntry>();
            RankPromotions = rankPromotions ?? Array.Empty<RankPromotionDocumentEntry>();
            Rewards = rewards ?? Array.Empty<RewardDocumentEntry>();
        }
    }

    internal sealed class RankDocumentEntry
    {
        public string Id { get; }
        public int RequiredExperience { get; }

        public RankDocumentEntry(string id, int requiredExperience)
        {
            Id = id;
            RequiredExperience = requiredExperience;
        }
    }

    internal sealed class CardDocumentEntry
    {
        public string Id { get; }
        public int ExperienceAmount { get; }

        public CardDocumentEntry(string id, int experienceAmount)
        {
            Id = id;
            ExperienceAmount = experienceAmount;
        }
    }

    internal sealed class QuestDocumentEntry
    {
        public string Id { get; }
        public string TitleLocalizationKey { get; }
        public string DescriptionLocalizationKey { get; }

        public QuestDocumentEntry(
            string id,
            string titleLocalizationKey,
            string descriptionLocalizationKey)
        {
            Id = id;
            TitleLocalizationKey = titleLocalizationKey;
            DescriptionLocalizationKey = descriptionLocalizationKey;
        }
    }

    internal sealed class RankPromotionDocumentEntry
    {
        public string RankId { get; }
        public string QuestId { get; }
        public string HeroLocalizationKey { get; }
        public int RequiredAmount { get; }
        public int DurationMinutes { get; }
        public PaymentDocumentEntry QuestPayment { get; }
        public PaymentDocumentEntry InstantPayment { get; }
        public string RewardId { get; }

        public RankPromotionDocumentEntry(
            string rankId,
            string questId,
            string heroLocalizationKey,
            int requiredAmount,
            int durationMinutes,
            PaymentDocumentEntry questPayment,
            PaymentDocumentEntry instantPayment,
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

    internal sealed class PaymentDocumentEntry
    {
        public string ItemId { get; }
        public int Amount { get; }

        public PaymentDocumentEntry(string itemId, int amount)
        {
            ItemId = itemId;
            Amount = amount;
        }
    }

    internal sealed class RewardDocumentEntry
    {
        public string Id { get; }
        public ItemAmountDocumentEntry[] Items { get; }

        public RewardDocumentEntry(string id, ItemAmountDocumentEntry[] items)
        {
            Id = id;
            Items = items ?? Array.Empty<ItemAmountDocumentEntry>();
        }
    }

    internal sealed class ItemAmountDocumentEntry
    {
        public string Id { get; }
        public int Amount { get; }

        public ItemAmountDocumentEntry(string id, int amount)
        {
            Id = id;
            Amount = amount;
        }
    }
}