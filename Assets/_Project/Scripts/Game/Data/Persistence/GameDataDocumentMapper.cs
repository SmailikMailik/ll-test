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
                document.RankUps.Select(ToDeclaration),
                document.Rewards.Select(reward =>
                    new RewardDeclaration(
                        reward.Id,
                        reward.Items.Select(item =>
                            new ItemAmountDeclaration(item.Id, item.Amount)))));
        }

        private static RankUpDeclaration ToDeclaration(RankUpDocumentEntry rankUp)
        {
            if (rankUp == null)
                throw new ArgumentException("Rank-up data cannot contain null entries.");

            return new RankUpDeclaration(
                rankUp.RankId,
                rankUp.QuestId,
                rankUp.HeroLocalizationKey,
                rankUp.RequiredAmount,
                rankUp.DurationMinutes,
                ToDeclaration(rankUp.QuestPayment),
                ToDeclaration(rankUp.InstantPayment),
                rankUp.RewardId);
        }

        private static PaymentDeclaration ToDeclaration(PaymentDocumentEntry payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            return new PaymentDeclaration(payment.ItemId, payment.Amount);
        }
    }
}