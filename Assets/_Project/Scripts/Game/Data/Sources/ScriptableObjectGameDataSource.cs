using System;
using System.Linq;
using LL.Game.Cards.Configuration;
using LL.Game.Data.Configuration;
using LL.Game.Data.Declarations;
using LL.Game.Heroes.Configuration;
using LL.Game.Payments.Configuration;
using LL.Game.RankUp.Configuration;
using LL.Game.Quests.Configuration;
using LL.Game.Ranks.Configuration;
using LL.Game.Rewards.Configuration;
using LL.Infrastructure.Loading;
using LL.Validation;

namespace LL.Game.Data.Sources
{
    internal sealed class ScriptableObjectGameDataSource : IDataSource<GameDataDeclaration>
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
                _manifest.Heroes.Heroes.Select(ToHeroDeclaration),
                _manifest.Quests.Quests.Select(ToQuestDeclaration),
                _manifest.RankUps.RankUps.Select(ToRankUpDeclaration),
                _manifest.Rewards.Rewards.Select(ToRewardDeclaration));
        }

        private void EnsureManifestIsValid()
        {
            ValidationRunner.EnsureValid(_manifest);
            ValidationRunner.EnsureValid(_manifest.Ranks);
            ValidationRunner.EnsureValid(_manifest.Cards);
            ValidationRunner.EnsureValid(_manifest.Heroes);
            ValidationRunner.EnsureValid(_manifest.Quests);
            ValidationRunner.EnsureValid(_manifest.RankUps);
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

        private static HeroDeclaration ToHeroDeclaration(HeroEntry hero)
        {
            return new HeroDeclaration(
                hero.Id.Value,
                hero.NameLocalizationKey,
                hero.FlagId.Value);
        }

        private static QuestDeclaration ToQuestDeclaration(QuestEntry quest)
        {
            return new QuestDeclaration(
                quest.Id.Value,
                quest.TitleLocalizationKey,
                quest.DescriptionLocalizationKey);
        }

        private static RankUpDeclaration ToRankUpDeclaration(RankUpEntry rankUp)
        {
            return new RankUpDeclaration(
                rankUp.RankId.Value,
                rankUp.QuestId.Value,
                rankUp.HeroId.Value,
                rankUp.RequiredCount,
                rankUp.DurationMinutes,
                ToPaymentDeclaration(rankUp.QuestPayment),
                ToPaymentDeclaration(rankUp.InstantPayment),
                rankUp.RewardId.Value);
        }

        private static PaymentDeclaration ToPaymentDeclaration(PaymentEntry payment)
        {
            return new PaymentDeclaration(payment.ItemId.Value, payment.Amount);
        }

        private static RewardDeclaration ToRewardDeclaration(RewardEntry reward)
        {
            return new RewardDeclaration(
                reward.Id.Value,
                reward.Items.Select(ToRewardItemDeclaration));
        }

        private static RewardItemDeclaration ToRewardItemDeclaration(RewardItemEntry item)
        {
            return new RewardItemDeclaration(item.Id.Value, item.Amount);
        }
    }
}