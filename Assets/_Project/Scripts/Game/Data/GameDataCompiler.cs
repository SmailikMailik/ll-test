using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Cards;
using LL.Game.Data.Declarations;
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
        private const string RewardReferenceCode = "rank-up.reward.exists";
        private const string FinalRankCode = "rank-up.rank.not-final";
        private const string RankUpRequiredCode = "rank-up.rank.required";

        public GameDataSnapshot Compile(GameDataDeclaration declaration)
        {
            if (declaration == null)
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
            var questIds = EnsureValidAndCollectIds(
                declaration.Quests,
                entry => new QuestId(entry.Id),
                nameof(declaration.Quests));
            var rewardIds = EnsureValidAndCollectIds(
                declaration.Rewards,
                entry => new RewardId(entry.Id),
                nameof(declaration.Rewards));
            var rankUpRankIds = EnsureValidAndCollectIds(
                declaration.RankUps,
                entry => new RankId(entry.RankId),
                nameof(declaration.RankUps));

            EnsureRankUpRelationsAreValid(
                declaration,
                rankIds,
                questIds,
                rewardIds,
                rankUpRankIds);

            var ranks = CompileRanks(declaration.Ranks);
            var cards = CompileCards(declaration.Cards);
            var quests = CompileQuests(declaration.Quests);
            var rankUps = CompileRankUps(declaration.RankUps);
            var rewards = CompileRewards(declaration.Rewards);

            return new GameDataSnapshot(ranks, cards, quests, rankUps, rewards);
        }

        private static RankCatalog CompileRanks(IReadOnlyList<RankDeclaration> declarations)
        {
            return new RankCatalog(declarations.Select(ToRankDefinition));
        }

        private static CardCatalog CompileCards(IReadOnlyList<CardDeclaration> declarations)
        {
            return new CardCatalog(declarations.Select(ToCard));
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
                new RankId(declaration.RankId),
                new RankUpQuest(
                    new QuestId(declaration.QuestId),
                    declaration.HeroLocalizationKey,
                    declaration.RequiredAmount,
                    TimeSpan.FromMinutes(declaration.DurationMinutes),
                    ToPayment(declaration.QuestPayment)),
                ToPayment(declaration.InstantPayment),
                new RewardId(declaration.RewardId));
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
            if (payment == null)
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
            ISet<RewardId> rewardIds,
            ISet<RankId> rankUpRankIds)
        {
            var finalRankId = new RankId(declaration.Ranks[declaration.Ranks.Count - 1].Id);

            ValidationRunner.EnsureValid(
                context =>
                {
                    var rankUpsContext = context.At(nameof(declaration.RankUps));

                    for (var index = 0; index < declaration.RankUps.Count; index++)
                    {
                        var rankUp = declaration.RankUps[index];
                        var rankUpContext = rankUpsContext.At(index);

                        ValidationRules.ReferenceExists(
                            new RankId(rankUp.RankId),
                            rankIds,
                            rankUpContext.At(nameof(rankUp.RankId)),
                            RankReferenceCode);
                        ValidationRules.ReferenceExists(
                            new QuestId(rankUp.QuestId),
                            questIds,
                            rankUpContext.At(nameof(rankUp.QuestId)),
                            QuestReferenceCode);
                        ValidationRules.ReferenceExists(
                            new RewardId(rankUp.RewardId),
                            rewardIds,
                            rankUpContext.At(nameof(rankUp.RewardId)),
                            RewardReferenceCode);
                        ValidationRules.NotEqual(
                            new RankId(rankUp.RankId),
                            finalRankId,
                            rankUpContext.At(nameof(rankUp.RankId)),
                            FinalRankCode);
                    }

                    for (var index = 0; index + 1 < declaration.Ranks.Count; index++)
                    {
                        ValidationRules.ReferenceExists(
                            new RankId(declaration.Ranks[index].Id),
                            rankUpRankIds,
                            rankUpsContext,
                            RankUpRequiredCode);
                    }
                },
                nameof(declaration));
        }
    }
}