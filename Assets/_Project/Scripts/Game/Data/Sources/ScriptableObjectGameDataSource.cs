using System;
using System.Linq;
using LL.Game.Cards.Configuration;
using LL.Game.Data.Configuration;
using LL.Game.Payments.Configuration;
using LL.Game.Promotions.Configuration;
using LL.Game.Quests.Configuration;
using LL.Game.Ranks.Configuration;
using LL.Game.Rewards.Configuration;
using LL.Validation;

namespace LL.Game.Data.Sources
{
    internal sealed class ScriptableObjectGameDataSource : IGameDataSource
    {
        private readonly GameDataManifestConfig _manifest;

        internal ScriptableObjectGameDataSource(GameDataManifestConfig manifest)
        {
            _manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
        }

        public GameDataDeclaration Read()
        {
            EnsureManifestIsValid();

            return new GameDataDeclaration(
                _manifest.Ranks.Ranks.Select(ToRankDeclaration),
                _manifest.Cards.Cards.Select(ToCardDeclaration),
                _manifest.Quests.Quests.Select(ToQuestDeclaration),
                _manifest.RankPromotions.Promotions.Select(ToRankPromotionDeclaration),
                _manifest.Rewards.Rewards.Select(ToRewardDeclaration));
        }

        private void EnsureManifestIsValid()
        {
            ValidationRunner.EnsureValid(_manifest);
            ValidationRunner.EnsureValid(_manifest.Ranks);
            ValidationRunner.EnsureValid(_manifest.Cards);
            ValidationRunner.EnsureValid(_manifest.Quests);
            ValidationRunner.EnsureValid(_manifest.RankPromotions);
            ValidationRunner.EnsureValid(_manifest.Rewards);
        }

        private static RankDeclaration ToRankDeclaration(RankEntry rank)
        {
            return new RankDeclaration(rank.Id.Value, rank.RequiredExperience);
        }

        private static CardDeclaration ToCardDeclaration(CardEntry card)
        {
            return new CardDeclaration(card.Id.Value, card.ExperienceAmount);
        }

        private static QuestDeclaration ToQuestDeclaration(QuestEntry quest)
        {
            return new QuestDeclaration(
                quest.Id.Value,
                quest.TitleLocalizationKey,
                quest.DescriptionLocalizationKey);
        }

        private static RankPromotionDeclaration ToRankPromotionDeclaration(RankPromotionEntry promotion)
        {
            return new RankPromotionDeclaration(
                promotion.RankId.Value,
                promotion.QuestId.Value,
                promotion.HeroLocalizationKey,
                promotion.RequiredAmount,
                promotion.DurationMinutes,
                ToPaymentDeclaration(promotion.QuestPayment),
                ToPaymentDeclaration(promotion.InstantPayment),
                promotion.RewardId.Value);
        }

        private static PaymentDeclaration ToPaymentDeclaration(PaymentEntry payment)
        {
            return new PaymentDeclaration(payment.ItemId.Value, payment.Amount);
        }

        private static RewardDeclaration ToRewardDeclaration(RewardEntry reward)
        {
            return new RewardDeclaration(
                reward.Id.Value,
                reward.Items.Select(ToItemAmountDeclaration));
        }

        private static ItemAmountDeclaration ToItemAmountDeclaration(RewardItemEntry item)
        {
            return new ItemAmountDeclaration(item.Id.Value, item.Amount);
        }
    }
}