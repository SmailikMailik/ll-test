using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Cards;
using LL.Game.Data.Declarations;
using LL.Game.Flags;
using LL.Game.Heroes;
using LL.Game.Items;
using LL.Game.Payments;
using LL.Game.Quests;
using LL.Game.Ranks;
using LL.Game.RankUp;
using LL.Game.Rewards;
using LL.Infrastructure.Compilation;

namespace LL.Game.Data
{
    internal sealed class GameDataCompiler : IDataCompiler<GameDataDeclaration, GameDataSnapshot>
    {
        public GameDataSnapshot Compile(GameDataDeclaration declaration)
        {
            GameDataDeclarationValidator.EnsureValid(declaration);

            return new GameDataSnapshot
            (
                CompileRanks(declaration.Ranks),
                CompileCards(declaration.Cards),
                CompileHeroes(declaration.Heroes),
                CompileQuests(declaration.Quests),
                CompileRankUps(declaration.RankUps),
                CompileRewards(declaration.Rewards)
            );
        }

        private static RankCatalog CompileRanks(IReadOnlyList<RankDeclaration> declarations)
        {
            return new RankCatalog(declarations.Select(ToRankDefinition));
        }

        private static CardCatalog CompileCards(IReadOnlyList<CardDeclaration> declarations)
        {
            return new CardCatalog(declarations.Select(ToCard));
        }

        private static HeroCatalog CompileHeroes(IReadOnlyList<HeroDeclaration> declarations)
        {
            return new HeroCatalog(declarations.Select(ToHeroDefinition));
        }

        private static QuestCatalog CompileQuests(IReadOnlyList<QuestDeclaration> declarations)
        {
            return new QuestCatalog(declarations.Select(ToQuestDefinition));
        }

        private static RankUpCatalog CompileRankUps(IReadOnlyList<RankUpDeclaration> declarations)
        {
            return new RankUpCatalog(declarations.Select(ToRankUpDefinition));
        }

        private static RewardCatalog CompileRewards(IReadOnlyList<RewardDeclaration> declarations)
        {
            return new RewardCatalog(declarations.Select(ToReward));
        }

        private static RankDefinition ToRankDefinition(RankDeclaration declaration, int index)
        {
            return new RankDefinition(
                new RankId(declaration.Id),
                index + 1,
                declaration.RequiredExperience);
        }

        private static Card ToCard(CardDeclaration declaration)
        {
            return new Card(new ItemId(declaration.Id), declaration.ExperienceAmount);
        }

        private static HeroDefinition ToHeroDefinition(HeroDeclaration declaration)
        {
            return new HeroDefinition(
                new HeroId(declaration.Id),
                declaration.NameLocalizationKey,
                new FlagId(declaration.FlagId));
        }

        private static QuestDefinition ToQuestDefinition(QuestDeclaration declaration)
        {
            return new QuestDefinition(
                new QuestId(declaration.Id),
                declaration.TitleLocalizationKey,
                declaration.DescriptionLocalizationKey);
        }

        private static RankUpDefinition ToRankUpDefinition(RankUpDeclaration declaration)
        {
            return new RankUpDefinition(
                new HeroId(declaration.HeroId),
                new RankId(declaration.RankId),
                declaration.Options.Select(ToRankUpOptionDefinition),
                new RewardId(declaration.RewardId));
        }

        private static RankUpOptionDefinition ToRankUpOptionDefinition(RankUpOptionDeclaration declaration)
        {
            return new RankUpOptionDefinition(
                new RankUpOptionId(declaration.OptionId),
                declaration.Requirements.Select(ToRankUpRequirementDefinition));
        }

        private static RankUpRequirementDefinition ToRankUpRequirementDefinition(
            RankUpRequirementDeclaration declaration)
        {
            return declaration switch
            {
                QuestRankUpRequirementDeclaration quest => ToQuestRankUpRequirementDefinition(quest),
                PaymentRankUpRequirementDeclaration payment => ToPaymentRankUpRequirementDefinition(payment),
                _ => throw new ArgumentException(
                    $"Unsupported rank-up requirement type '{declaration?.GetType().Name}'.",
                    nameof(declaration))
            };
        }

        private static QuestRankUpRequirementDefinition ToQuestRankUpRequirementDefinition(
            QuestRankUpRequirementDeclaration declaration)
        {
            return new QuestRankUpRequirementDefinition(
                new RankUpRequirementId(declaration.RequirementId),
                new QuestId(declaration.QuestId),
                declaration.RequiredCount,
                TimeSpan.FromMinutes(declaration.DurationMinutes));
        }

        private static PaymentRankUpRequirementDefinition ToPaymentRankUpRequirementDefinition(
            PaymentRankUpRequirementDeclaration declaration)
        {
            return new PaymentRankUpRequirementDefinition(
                new RankUpRequirementId(declaration.RequirementId),
                ToPayment(declaration.Payment));
        }

        private static Reward ToReward(RewardDeclaration declaration)
        {
            return new Reward(
                new RewardId(declaration.Id),
                declaration.Items.Select(ToItemAmount));
        }

        private static ItemAmount ToItemAmount(RewardItemDeclaration declaration)
        {
            return new ItemAmount(new ItemId(declaration.Id), declaration.Amount);
        }

        private static Payment ToPayment(PaymentDeclaration payment)
        {
            if (payment is null)
                throw new ArgumentNullException(nameof(payment));

            return new Payment(new ItemId(payment.ItemId), payment.Amount);
        }
    }
}