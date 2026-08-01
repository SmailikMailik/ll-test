using System;
using System.Linq;
using LL.Game.Data.Declarations;
using LL.Game.Data.Persistence.Documents;

namespace LL.Game.Data.Persistence
{
    internal static class GameDataDocumentMapper
    {
        internal static GameDataDeclaration ToDeclaration(GameDataDocument document)
        {
            if (document is null)
                throw new ArgumentNullException(nameof(document));

            return new GameDataDeclaration(
                document.Ranks.Select(ToRankDeclaration),
                document.Cards.Select(ToCardDeclaration),
                document.Heroes.Select(ToHeroDeclaration),
                document.Quests.Select(ToQuestDeclaration),
                document.RankUps.Select(ToRankUpDeclaration),
                document.Rewards.Select(ToRewardDeclaration));
        }

        private static RankDeclaration ToRankDeclaration(RankDocumentEntry rank)
        {
            return new RankDeclaration(rank.Id, rank.RequiredExperience);
        }

        private static CardDeclaration ToCardDeclaration(CardDocumentEntry card)
        {
            return new CardDeclaration(card.Id, card.ExperienceAmount);
        }

        private static HeroDeclaration ToHeroDeclaration(HeroDocumentEntry hero)
        {
            return new HeroDeclaration(hero.Id, hero.NameLocalizationKey, hero.FlagId);
        }

        private static QuestDeclaration ToQuestDeclaration(QuestDocumentEntry quest)
        {
            return new QuestDeclaration(
                quest.Id,
                quest.TitleLocalizationKey,
                quest.DescriptionLocalizationKey);
        }

        private static RankUpDeclaration ToRankUpDeclaration(RankUpDocumentEntry rankUp)
        {
            if (rankUp is null)
                throw new ArgumentException("Rank-up data cannot contain null entries.");

            return new RankUpDeclaration(
                rankUp.HeroId,
                rankUp.RankId,
                rankUp.RewardId,
                rankUp.Options.Select(ToRankUpOptionDeclaration));
        }

        private static RankUpOptionDeclaration ToRankUpOptionDeclaration(RankUpOptionDocumentEntry option)
        {
            return new RankUpOptionDeclaration(
                option.OptionId,
                option.Quests.Select(ToQuestRankUpRequirementDeclaration),
                option.Payments.Select(ToPaymentRankUpRequirementDeclaration));
        }

        private static QuestRankUpRequirementDeclaration ToQuestRankUpRequirementDeclaration(
            QuestRankUpRequirementDocumentEntry quest)
        {
            return new QuestRankUpRequirementDeclaration(
                quest.RequirementId,
                quest.QuestId,
                quest.RequiredCount,
                quest.DurationMinutes);
        }

        private static PaymentRankUpRequirementDeclaration ToPaymentRankUpRequirementDeclaration(
            PaymentRankUpRequirementDocumentEntry payment)
        {
            return new PaymentRankUpRequirementDeclaration(
                payment.RequirementId,
                ToPaymentDeclaration(payment.Payment));
        }

        private static PaymentDeclaration ToPaymentDeclaration(PaymentDocumentEntry payment)
        {
            if (payment is null)
                throw new ArgumentNullException(nameof(payment));

            return new PaymentDeclaration(payment.ItemId, payment.Amount);
        }

        private static RewardDeclaration ToRewardDeclaration(RewardDocumentEntry reward)
        {
            return new RewardDeclaration(
                reward.Id,
                reward.Items.Select(ToRewardItemDeclaration));
        }

        private static RewardItemDeclaration ToRewardItemDeclaration(RewardItemDocumentEntry item)
        {
            return new RewardItemDeclaration(item.Id, item.Amount);
        }
    }
}