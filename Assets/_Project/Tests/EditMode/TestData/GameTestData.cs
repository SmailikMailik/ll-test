using System;
using LL.Game.Data;
using LL.Game.Data.Declarations;
using LL.Game.Items;
using LL.Game.RankUp;

namespace LL.Tests.EditMode.TestData
{
    internal static class GameTestData
    {
        internal static GameDataDeclaration CreateDeclaration(string rewardId = "reward") => new
        (
            CreateRanks(),
            CreateCards(),
            CreateHeroes(),
            CreateQuests(),
            CreateRankUps(rewardId),
            CreateRewards()
        );

        internal static GameDataSnapshot CreateSnapshot()
        {
            return new GameDataCompiler().Compile(CreateDeclaration());
        }

        internal static RankUpCatalog CreateRankUpCatalog()
        {
            return CreateSnapshot().RankUps;
        }

        private static RankDeclaration[] CreateRanks()
        {
            return new[]
            {
                new RankDeclaration("bronze", 0),
                new RankDeclaration("silver", 100)
            };
        }

        private static CardDeclaration[] CreateCards()
        {
            return new[] { new CardDeclaration(ItemIds.Soft.Value, 10) };
        }

        private static HeroDeclaration[] CreateHeroes()
        {
            return new[] { new HeroDeclaration("hero", "hero.name", "flag") };
        }

        private static QuestDeclaration[] CreateQuests()
        {
            return new[] { new QuestDeclaration("quest", "quest.title", "quest.description") };
        }

        private static RankUpDeclaration[] CreateRankUps(string rewardId)
        {
            return new[]
            {
                new RankUpDeclaration(
                    "hero",
                    "bronze",
                    rewardId,
                    CreateRankUpOptions())
            };
        }

        private static RankUpOptionDeclaration[] CreateRankUpOptions()
        {
            return new[]
            {
                CreateQuestRankUpOption(),
                CreateInstantRankUpOption(),
                CreateFreeRankUpOption()
            };
        }

        private static RankUpOptionDeclaration CreateQuestRankUpOption()
        {
            return new RankUpOptionDeclaration(
                "quest",
                new RankUpRequirementDeclaration[]
                {
                    new QuestRankUpRequirementDeclaration("quest", "quest", 1, 10),
                    new PaymentRankUpRequirementDeclaration(
                        "soft-payment",
                        new PaymentDeclaration(ItemIds.Soft.Value, 1))
                });
        }

        private static RankUpOptionDeclaration CreateInstantRankUpOption()
        {
            return new RankUpOptionDeclaration(
                "instant",
                new RankUpRequirementDeclaration[]
                {
                    new PaymentRankUpRequirementDeclaration(
                        "hard-payment",
                        new PaymentDeclaration(ItemIds.Hard.Value, 1))
                });
        }

        private static RankUpOptionDeclaration CreateFreeRankUpOption()
        {
            return new RankUpOptionDeclaration(
                "free",
                Array.Empty<RankUpRequirementDeclaration>());
        }

        private static RewardDeclaration[] CreateRewards()
        {
            return new[]
            {
                new RewardDeclaration(
                    "reward",
                    new[] { new RewardItemDeclaration(ItemIds.MasterPoint.Value, 1) })
            };
        }
    }
}