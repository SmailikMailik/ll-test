using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Cards;
using LL.Game.Data;
using LL.Game.Data.Declarations;
using LL.Game.Flags;
using LL.Game.Heroes;
using LL.Game.Items;
using LL.Game.Payments;
using LL.Game.Quests;
using LL.Game.Ranks;
using LL.Game.RankUp;
using LL.Game.Rewards;
using LL.User.Defaults;
using LL.User.Defaults.Declarations;
using LL.User.Snapshots;

namespace LL.Tests.EditMode
{
    internal static class TestDataFactory
    {
        internal static GameDataDeclaration CreateGameDataDeclaration(string rewardId = "reward")
        {
            return new GameDataDeclaration(
                new[]
                {
                    new RankDeclaration("bronze", 0),
                    new RankDeclaration("silver", 100)
                },
                new[] { new CardDeclaration(ItemIds.Soft.Value, 10) },
                new[] { new HeroDeclaration("hero", "hero.name", "flag") },
                new[] { new QuestDeclaration("quest", "quest.title", "quest.description") },
                new[]
                {
                    new RankUpDeclaration(
                        "hero",
                        "bronze",
                        rewardId,
                        new[]
                        {
                            new RankUpOptionDeclaration(
                                "quest",
                                new RankUpRequirementDeclaration[]
                                {
                                    new QuestRankUpRequirementDeclaration("quest", "quest", 1, 10),
                                    new PaymentRankUpRequirementDeclaration(
                                        "soft-payment",
                                        new PaymentDeclaration(ItemIds.Soft.Value, 1))
                                }),
                            new RankUpOptionDeclaration(
                                "instant",
                                new RankUpRequirementDeclaration[]
                                {
                                    new PaymentRankUpRequirementDeclaration(
                                        "hard-payment",
                                        new PaymentDeclaration(ItemIds.Hard.Value, 1))
                                }),
                            new RankUpOptionDeclaration(
                                "free",
                                Array.Empty<RankUpRequirementDeclaration>())
                        })
                },
                new[]
                {
                    new RewardDeclaration(
                        "reward",
                        new[] { new RewardItemDeclaration(ItemIds.MasterPoint.Value, 1) })
                });
        }

        internal static GameDataSnapshot CreateGameDataSnapshot()
        {
            return new GameDataCompiler().Compile(CreateGameDataDeclaration());
        }

        internal static UserDefaultsSnapshot CreateUserDefaults(GameDataSnapshot gameData)
        {
            var declaration = new UserDefaultsDeclaration(
                new UserIdentityDefaultDeclaration("test-user", "RU"),
                new UserHeroSelectionDefaultDeclaration("hero"),
                new[] { new UserHeroDefaultDeclaration("hero", "bronze", 0) },
                ItemIds.All.Select(id => new UserItemDefaultDeclaration(id.Value, 0)));
            var compiler = new UserDefaultsCompiler(
                gameData.Ranks,
                gameData.Heroes,
                gameData.Cards,
                gameData.RankUps,
                gameData.Rewards);

            return compiler.Compile(declaration);
        }

        internal static UserSnapshot CreateUserSnapshot(
            RankId? rankId = null,
            HeroId? heroId = null,
            int experience = 0,
            IReadOnlyList<UserRankUpAttemptSnapshot> rankUpAttempts = null,
            params ItemAmount[] items)
        {
            var amounts = items.Length > 0
                ? items
                : ItemIds.All.Select(id => new ItemAmount(id, 0)).ToArray();

            return new UserSnapshot(
                new UserIdentitySnapshot("test-user", "RU"),
                new UserHeroSelectionSnapshot(heroId ?? new HeroId("hero")),
                new UserHeroesSnapshot(
                    new[]
                    {
                        new UserHeroSnapshot(
                            heroId ?? new HeroId("hero"),
                            new UserProgressSnapshot(rankId ?? new RankId("bronze"), experience),
                            rankUpAttempts ?? Array.Empty<UserRankUpAttemptSnapshot>())
                    }),
                new UserItemsSnapshot(amounts));
        }

        internal static RankUpCatalog CreateRankUpCatalog()
        {
            return new RankUpCatalog(
                new[]
                {
                    new RankUpDefinition(
                        new HeroId("hero"),
                        new RankId("bronze"),
                        new[]
                        {
                            new RankUpOptionDefinition(
                                new RankUpOptionId("quest"),
                                new RankUpRequirementDefinition[]
                                {
                                    new QuestRankUpRequirementDefinition(
                                        new RankUpRequirementId("quest"),
                                        new QuestId("quest"),
                                        1,
                                        TimeSpan.FromMinutes(10d)),
                                    new PaymentRankUpRequirementDefinition(
                                        new RankUpRequirementId("soft-payment"),
                                        new Payment(ItemIds.Soft, 1))
                                }),
                            new RankUpOptionDefinition(
                                new RankUpOptionId("instant"),
                                new RankUpRequirementDefinition[]
                                {
                                    new PaymentRankUpRequirementDefinition(
                                        new RankUpRequirementId("hard-payment"),
                                        new Payment(ItemIds.Hard, 1))
                                })
                        },
                        new RewardId("reward"))
                });
        }
    }

    internal sealed class ManualTimeProvider : TimeProvider
    {
        private DateTimeOffset _utcNow;

        internal ManualTimeProvider(DateTimeOffset utcNow)
        {
            _utcNow = utcNow;
        }

        public override DateTimeOffset GetUtcNow() => _utcNow;

        internal void Advance(TimeSpan duration)
        {
            _utcNow = _utcNow.Add(duration);
        }
    }
}