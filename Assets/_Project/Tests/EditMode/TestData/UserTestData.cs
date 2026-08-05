using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Data;
using LL.Game.Heroes;
using LL.Game.Items;
using LL.Game.Ranks;
using LL.User.Defaults;
using LL.User.Defaults.Declarations;
using LL.User.Snapshots;

namespace LL.Tests.EditMode.TestData
{
    internal static class UserTestData
    {
        internal static UserDefaultsDeclaration CreateDefaultsDeclaration(
            string selectedHeroId = "hero",
            IEnumerable<ItemId> itemIds = null)
        {
            var items = itemIds ?? ItemIds.All;

            return new UserDefaultsDeclaration(
                new UserIdentityDefaultDeclaration("test-user", "RU"),
                new UserHeroSelectionDefaultDeclaration(selectedHeroId),
                new[] { new UserHeroDefaultDeclaration("hero", "bronze", 0) },
                items.Select(id => new UserItemDefaultDeclaration(id.Value, 0)));
        }

        internal static UserDefaultsSnapshot CreateDefaults(GameDataSnapshot gameData)
        {
            var compiler = new UserDefaultsCompiler(
                gameData.Ranks,
                gameData.Heroes,
                gameData.Cards,
                gameData.RankUps,
                gameData.Rewards);

            return compiler.Compile(CreateDefaultsDeclaration());
        }

        internal static UserSnapshot CreateSnapshot(
            RankId? rankId = null,
            HeroId? heroId = null,
            int experience = 0,
            IReadOnlyList<UserRankUpAttemptSnapshot> rankUpAttempts = null,
            params ItemAmount[] items)
        {
            var resolvedHeroId = heroId ?? new HeroId("hero");
            var amounts = items.Length > 0
                ? items
                : ItemIds.All.Select(id => new ItemAmount(id, 0)).ToArray();

            return new UserSnapshot(
                new UserIdentitySnapshot("test-user", "RU"),
                new UserHeroSelectionSnapshot(resolvedHeroId),
                new UserHeroesSnapshot(
                    new[]
                    {
                        new UserHeroSnapshot(
                            resolvedHeroId,
                            new UserProgressSnapshot(rankId ?? new RankId("bronze"), experience),
                            rankUpAttempts ?? Array.Empty<UserRankUpAttemptSnapshot>())
                    }),
                new UserItemsSnapshot(amounts));
        }
    }
}