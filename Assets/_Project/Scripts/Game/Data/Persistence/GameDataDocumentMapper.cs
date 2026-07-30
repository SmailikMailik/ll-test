using System;
using System.Linq;

namespace LL.Game.Data.Persistence
{
    internal static class GameDataDocumentMapper
    {
        internal static GameDataDeclaration ToDeclaration(GameDataDocument document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            return new GameDataDeclaration(
                document.Ranks.Select(rank =>
                    new RankDeclaration(rank.Id, rank.RequiredExperience)),
                document.Cards.Select(card =>
                    new CardDeclaration(card.Id, card.ExperienceAmount)),
                document.Quests.Select(quest =>
                    new QuestDeclaration(
                        quest.Id,
                        quest.TitleLocalizationKey,
                        quest.DescriptionLocalizationKey)),
                document.RankPromotions.Select(ToDeclaration),
                document.Rewards.Select(reward =>
                    new RewardDeclaration(
                        reward.Id,
                        reward.Items.Select(item =>
                            new ItemAmountDeclaration(item.Id, item.Amount)))));
        }

        private static RankPromotionDeclaration ToDeclaration(RankPromotionDocumentEntry promotion)
        {
            if (promotion == null)
                throw new ArgumentException("Rank promotion data cannot contain null entries.");

            return new RankPromotionDeclaration(
                promotion.RankId,
                promotion.QuestId,
                promotion.HeroLocalizationKey,
                promotion.RequiredAmount,
                promotion.DurationMinutes,
                ToDeclaration(promotion.QuestPayment),
                ToDeclaration(promotion.InstantPayment),
                promotion.RewardId);
        }

        private static PaymentDeclaration ToDeclaration(PaymentDocumentEntry payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            return new PaymentDeclaration(payment.ItemId, payment.Amount);
        }
    }
}