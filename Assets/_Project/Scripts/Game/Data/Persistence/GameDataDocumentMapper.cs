using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Cards;
using LL.Game.Items;
using LL.Game.Payments;
using LL.Game.Promotions;
using LL.Game.Quests;
using LL.Game.Ranks;
using LL.Game.Rewards;

namespace LL.Game.Data.Persistence
{
    internal static class GameDataDocumentMapper
    {
        internal static GameDataSnapshot ToSnapshot(GameDataDocument document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            EnsureReferencesAreValid(document);

            return new GameDataSnapshot(
                new RankCatalog(
                    document.Ranks.Select((rank, index) =>
                        new RankDefinition(
                            new RankId(rank.Id),
                            index + 1,
                            rank.RequiredExperience))),
                new CardCatalog(
                    document.Cards.Select(card =>
                        new Card(new ItemId(card.Id), card.ExperienceAmount))),
                new QuestCatalog(
                    document.Quests.Select(quest =>
                        new QuestDefinition(
                            new QuestId(quest.Id),
                            quest.TitleLocalizationKey,
                            quest.DescriptionLocalizationKey))),
                new RankPromotionCatalog(
                    document.RankPromotions.Select(ToRankPromotion)),
                new RewardCatalog(
                    document.Rewards.Select(reward =>
                        new Reward(
                            new RewardId(reward.Id),
                            reward.Items.Select(item =>
                                new ItemAmount(new ItemId(item.Id), item.Amount))))));
        }

        private static RankPromotion ToRankPromotion(RankPromotionDocumentEntry promotion)
        {
            if (promotion == null)
                throw new ArgumentException("Rank promotion data cannot contain null entries.");

            return new RankPromotion(
                new RankId(promotion.RankId),
                new RankPromotionQuest(
                    new QuestId(promotion.QuestId),
                    promotion.HeroLocalizationKey,
                    promotion.RequiredAmount,
                    TimeSpan.FromMinutes(promotion.DurationMinutes),
                    ToPayment(promotion.QuestPayment)),
                ToPayment(promotion.InstantPayment),
                new RewardId(promotion.RewardId));
        }

        private static Payment ToPayment(PaymentDocumentEntry payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            return new Payment(new ItemId(payment.ItemId), payment.Amount);
        }

        private static void EnsureReferencesAreValid(GameDataDocument document)
        {
            var rankIds = CollectIds(document.Ranks, rank => rank?.Id, nameof(document.Ranks));
            CollectIds(document.Cards, card => card?.Id, nameof(document.Cards));
            var questIds = CollectIds(document.Quests, quest => quest?.Id, nameof(document.Quests));
            var rewardIds = CollectIds(document.Rewards, reward => reward?.Id, nameof(document.Rewards));

            foreach (var promotion in document.RankPromotions)
            {
                if (promotion == null)
                    throw new ArgumentException("Rank promotions cannot contain null entries.");

                EnsureReferenceExists(
                    rankIds,
                    promotion.RankId,
                    nameof(promotion.RankId));
                EnsureReferenceExists(
                    questIds,
                    promotion.QuestId,
                    nameof(promotion.QuestId));
                EnsureReferenceExists(
                    rewardIds,
                    promotion.RewardId,
                    nameof(promotion.RewardId));
            }
        }

        private static HashSet<string> CollectIds<TData>(
            IEnumerable<TData> entries,
            Func<TData, string> idSelector,
            string collectionName)
        {
            var ids = new HashSet<string>(StringComparer.Ordinal);

            foreach (var entry in entries)
            {
                var id = idSelector(entry);

                if (string.IsNullOrWhiteSpace(id))
                    throw new ArgumentException($"{collectionName} contains an empty ID.");

                if (ids.Add(id) is false)
                    throw new ArgumentException($"{collectionName} contains duplicate ID '{id}'.");
            }

            return ids;
        }

        private static void EnsureReferenceExists(
            ISet<string> ids,
            string id,
            string referenceName)
        {
            if (ids.Contains(id) is false)
                throw new ArgumentException($"{referenceName} references unknown ID '{id}'.");
        }
    }
}