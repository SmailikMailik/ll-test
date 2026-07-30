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

namespace LL.Game.Data
{
    internal sealed class GameDataCompiler
    {
        internal GameDataSnapshot Compile(GameDataDeclaration declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            var rankIds = CollectIds(declaration.Ranks, entry => entry?.Id, nameof(declaration.Ranks));
            CollectIds(declaration.Cards, entry => entry?.Id, nameof(declaration.Cards));
            var questIds = CollectIds(declaration.Quests, entry => entry?.Id, nameof(declaration.Quests));
            var rewardIds = CollectIds(declaration.Rewards, entry => entry?.Id, nameof(declaration.Rewards));

            foreach (var promotion in declaration.RankPromotions)
            {
                if (promotion == null)
                    throw new ArgumentException("Rank promotions cannot contain null entries.", nameof(declaration));

                EnsureReferenceExists(rankIds, promotion.RankId, nameof(promotion.RankId));
                EnsureReferenceExists(questIds, promotion.QuestId, nameof(promotion.QuestId));
                EnsureReferenceExists(rewardIds, promotion.RewardId, nameof(promotion.RewardId));
            }

            return new GameDataSnapshot(
                new RankCatalog(declaration.Ranks.Select((rank, index) =>
                    new RankDefinition(new RankId(rank.Id), index + 1, rank.RequiredExperience))),
                new CardCatalog(declaration.Cards.Select(card =>
                    new Card(new ItemId(card.Id), card.ExperienceAmount))),
                new QuestCatalog(declaration.Quests.Select(quest =>
                    new QuestDefinition(
                        new QuestId(quest.Id),
                        quest.TitleLocalizationKey,
                        quest.DescriptionLocalizationKey))),
                new RankPromotionCatalog(declaration.RankPromotions.Select(ToRankPromotion)),
                new RewardCatalog(declaration.Rewards.Select(reward =>
                    new Reward(
                        new RewardId(reward.Id),
                        reward.Items.Select(item => new ItemAmount(new ItemId(item.Id), item.Amount))))));
        }

        private static RankPromotion ToRankPromotion(RankPromotionDeclaration promotion)
        {
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

        private static Payment ToPayment(PaymentDeclaration payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            return new Payment(new ItemId(payment.ItemId), payment.Amount);
        }

        private static HashSet<string> CollectIds<T>(
            IEnumerable<T> entries,
            Func<T, string> idSelector,
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

        private static void EnsureReferenceExists(ISet<string> ids, string id, string referenceName)
        {
            if (ids.Contains(id) is false)
                throw new ArgumentException($"{referenceName} references unknown ID '{id}'.");
        }
    }
}