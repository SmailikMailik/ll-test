using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Cards;
using LL.Game.Data.Declarations;
using LL.Game.Flags;
using LL.Game.Heroes;
using LL.Game.Identifiers;
using LL.Game.Items;
using LL.Game.Payments;
using LL.Game.RankUp;
using LL.Game.Quests;
using LL.Game.Ranks;
using LL.Game.Rewards;
using LL.Infrastructure.Compilation;
using LL.Validation;

namespace LL.Game.Data
{
    internal sealed class GameDataCompiler : IDataCompiler<GameDataDeclaration, GameDataSnapshot>
    {
        private const string RankEntriesCode = "rank-catalog.entries.not-empty";
        private const string RankReferenceCode = "rank-up.rank.exists";
        private const string QuestReferenceCode = "rank-up.quest.exists";
        private const string HeroReferenceCode = "rank-up.hero.exists";
        private const string RewardReferenceCode = "rank-up.reward.exists";
        private const string FinalRankCode = "rank-up.rank.not-final";
        private const string RankUpRequiredCode = "rank-up.rank.required";
        private const string RankUpUniqueCode = "rank-up.hero-rank.unique";
        private const string OptionsCode = "rank-up.options.not-empty";
        private const string OptionUniqueCode = "rank-up.option.unique";
        private const string RequirementUniqueCode = "rank-up.requirement.unique";
        private const string RequirementTypeCode = "rank-up.requirement.type.supported";
        private const string RequiredCountCode = "rank-up.quest.count.positive";
        private const string DurationCode = "rank-up.quest.duration.positive";

        public GameDataSnapshot Compile(GameDataDeclaration declaration)
        {
            if (declaration is null)
                throw new ArgumentNullException(nameof(declaration));

            ValidationRunner.EnsureValid(
                context => ValidationRules.NotEmpty(
                    declaration.Ranks,
                    context.At(nameof(declaration.Ranks)),
                    RankEntriesCode),
                nameof(declaration));

            var rankIds = EnsureValidAndCollectIds(
                declaration.Ranks,
                entry => new RankId(entry.Id),
                nameof(declaration.Ranks));
            EnsureValidIds(
                declaration.Cards,
                entry => new ItemId(entry.Id),
                nameof(declaration.Cards));
            var heroIds = EnsureValidAndCollectIds(
                declaration.Heroes,
                entry => new HeroId(entry.Id),
                nameof(declaration.Heroes));
            var questIds = EnsureValidAndCollectIds(
                declaration.Quests,
                entry => new QuestId(entry.Id),
                nameof(declaration.Quests));
            var rewardIds = EnsureValidAndCollectIds(
                declaration.Rewards,
                entry => new RewardId(entry.Id),
                nameof(declaration.Rewards));
            EnsureRankUpRelationsAreValid(
                declaration,
                rankIds,
                questIds,
                heroIds,
                rewardIds);

            var ranks = CompileRanks(declaration.Ranks);
            var cards = CompileCards(declaration.Cards);
            var heroes = CompileHeroes(declaration.Heroes);
            var quests = CompileQuests(declaration.Quests);
            var rankUps = CompileRankUps(declaration.RankUps);
            var rewards = CompileRewards(declaration.Rewards);

            return new GameDataSnapshot(ranks, cards, heroes, quests, rankUps, rewards);
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

        private static HashSet<TId> EnsureValidAndCollectIds<TEntry, TId>(
            IEnumerable<TEntry> entries,
            Func<TEntry, TId> getId,
            string collectionName)
            where TId : struct, IIdentifier
        {
            IdentifierCollectionValidator.EnsureValid(entries, getId, collectionName);
            return entries.Select(getId).ToHashSet();
        }

        private static void EnsureValidIds<TEntry, TId>(
            IEnumerable<TEntry> entries,
            Func<TEntry, TId> getId,
            string collectionName)
            where TId : struct, IIdentifier
        {
            IdentifierCollectionValidator.EnsureValid(entries, getId, collectionName);
        }

        private static void EnsureRankUpRelationsAreValid(
            GameDataDeclaration declaration,
            ISet<RankId> rankIds,
            ISet<QuestId> questIds,
            ISet<HeroId> heroIds,
            ISet<RewardId> rewardIds)
        {
            var finalRankId = new RankId(declaration.Ranks[declaration.Ranks.Count - 1].Id);

            ValidationRunner.EnsureValid(
                context =>
                {
                    var rankUpsContext = context.At(nameof(declaration.RankUps));
                    var rankUpKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                    for (var index = 0; index < declaration.RankUps.Count; index++)
                    {
                        var rankUp = declaration.RankUps[index];
                        var rankUpContext = rankUpsContext.At(index);

                        if (ValidationRules.NotNull(rankUp, rankUpContext, RankUpRequiredCode) is false)
                            continue;

                        var heroId = new HeroId(rankUp.HeroId);
                        var rankId = new RankId(rankUp.RankId);
                        var rewardId = new RewardId(rankUp.RewardId);

                        ValidationRules.ReferenceExists(
                            rankId,
                            rankIds,
                            rankUpContext.At(nameof(rankUp.RankId)),
                            RankReferenceCode);
                        ValidationRules.ReferenceExists(
                            heroId,
                            heroIds,
                            rankUpContext.At(nameof(rankUp.HeroId)),
                            HeroReferenceCode);
                        ValidationRules.ReferenceExists(
                            rewardId,
                            rewardIds,
                            rankUpContext.At(nameof(rankUp.RewardId)),
                            RewardReferenceCode);
                        ValidationRules.NotEqual(
                            rankId,
                            finalRankId,
                            rankUpContext.At(nameof(rankUp.RankId)),
                            FinalRankCode);

                        ValidationRules.TryAddUnique(
                            $"{heroId.Value}\n{rankId.Value}",
                            rankUpKeys,
                            rankUpContext,
                            RankUpUniqueCode);

                        ValidateRankUpOptions(rankUp, questIds, rankUpContext);
                    }

                    foreach (var heroId in heroIds)
                    {
                        for (var index = 0; index + 1 < declaration.Ranks.Count; index++)
                        {
                            var rankId = new RankId(declaration.Ranks[index].Id);
                            ValidationRules.ReferenceExists(
                                $"{heroId.Value}\n{rankId.Value}",
                                rankUpKeys,
                                rankUpsContext,
                                RankUpRequiredCode);
                        }
                    }
                },
                nameof(declaration));
        }

        private static void ValidateRankUpOptions(
            RankUpDeclaration rankUp,
            ISet<QuestId> questIds,
            ValidationContext context)
        {
            var optionsContext = context.At(nameof(rankUp.Options));

            if (ValidationRules.NotEmpty(rankUp.Options, optionsContext, OptionsCode) is false)
                return;

            var optionIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (var optionIndex = 0; optionIndex < rankUp.Options.Count; optionIndex++)
            {
                var option = rankUp.Options[optionIndex];
                var optionContext = optionsContext.At(optionIndex);

                if (ValidationRules.NotNull(option, optionContext, OptionsCode) is false)
                    continue;

                var optionId = new RankUpOptionId(option.OptionId);
                IdentifierValidator.Validate(optionId, optionContext.At(nameof(option.OptionId)));
                ValidationRules.TryAddUnique(optionId.Value, optionIds, optionContext, OptionUniqueCode);
                ValidateRankUpRequirements(option, questIds, optionContext);
            }
        }

        private static void ValidateRankUpRequirements(
            RankUpOptionDeclaration option,
            ISet<QuestId> questIds,
            ValidationContext context)
        {
            var requirementIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (var index = 0; index < option.Requirements.Count; index++)
            {
                var requirement = option.Requirements[index];
                var requirementContext = context.At(nameof(option.Requirements)).At(index);

                if (ValidationRules.NotNull(requirement, requirementContext, OptionsCode) is false)
                    continue;

                ValidateRequirementId(requirement.RequirementId, requirementIds, requirementContext);
                ValidateRankUpRequirement(requirement, questIds, requirementContext);
            }
        }

        private static void ValidateRankUpRequirement(
            RankUpRequirementDeclaration requirement,
            ISet<QuestId> questIds,
            ValidationContext context)
        {
            switch (requirement)
            {
                case QuestRankUpRequirementDeclaration quest:
                    ValidationRules.ReferenceExists(
                        new QuestId(quest.QuestId),
                        questIds,
                        context.At(nameof(quest.QuestId)),
                        QuestReferenceCode);
                    ValidationRules.Positive(
                        quest.RequiredCount,
                        context.At(nameof(quest.RequiredCount)),
                        RequiredCountCode);
                    ValidationRules.Positive(
                        quest.DurationMinutes,
                        context.At(nameof(quest.DurationMinutes)),
                        DurationCode);
                    break;
                case PaymentRankUpRequirementDeclaration payment:
                    ToPayment(payment.Payment);
                    break;
                default:
                    context.Report(
                        ValidationSeverity.Error,
                        RequirementTypeCode,
                        $"Unsupported rank-up requirement type '{requirement.GetType().Name}'.");
                    break;
            }
        }

        private static void ValidateRequirementId(
            string value,
            ISet<string> usedIds,
            ValidationContext context)
        {
            var requirementId = new RankUpRequirementId(value);
            IdentifierValidator.Validate(requirementId, context.At("RequirementId"));
            ValidationRules.TryAddUnique(value, usedIds, context, RequirementUniqueCode);
        }
    }
}