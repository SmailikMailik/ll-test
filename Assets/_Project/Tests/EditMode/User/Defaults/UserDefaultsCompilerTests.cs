using System;
using System.Linq;
using LL.Game.Data;
using LL.Game.Heroes;
using LL.Game.Items;
using LL.Tests.EditMode.TestData;
using LL.User.Defaults;
using NUnit.Framework;

namespace LL.Tests.EditMode.User.Defaults
{
    internal sealed class UserDefaultsCompilerTests
    {
        [Test]
        public void ValidDeclarationBuildsDefaults()
        {
            var gameData = GameTestData.CreateSnapshot();
            var defaults = UserTestData.CreateDefaults(gameData);
            var snapshot = defaults.CreateUserSnapshot();

            Assert.That(snapshot.Identity.UserId, Is.EqualTo("test-user"));
            Assert.That(snapshot.HeroSelection.HeroId, Is.EqualTo(new HeroId("hero")));
            Assert.That(snapshot.Heroes.TryGetHero(new HeroId("hero"), out var hero), Is.True);
            Assert.That(hero.Progress.RankId.Value, Is.EqualTo("bronze"));
            Assert.That(snapshot.Items.Amounts, Has.Count.EqualTo(ItemIds.All.Count));
        }

        [Test]
        public void MissingRequiredItemIsRejected()
        {
            var gameData = GameTestData.CreateSnapshot();
            var compiler = CreateCompiler(gameData);
            var declaration = UserTestData.CreateDefaultsDeclaration(
                itemIds: ItemIds.All
                    .Where(id => id.Equals(ItemIds.Hard) is false)
                    .ToArray());

            Assert.Throws<ArgumentException>(() => compiler.Compile(declaration));
        }

        [Test]
        public void UnknownDefaultHeroIsRejected()
        {
            var gameData = GameTestData.CreateSnapshot();
            var compiler = CreateCompiler(gameData);
            var declaration = UserTestData.CreateDefaultsDeclaration(selectedHeroId: "unknown");

            Assert.Throws<ArgumentException>(() => compiler.Compile(declaration));
        }

        private static UserDefaultsCompiler CreateCompiler(GameDataSnapshot gameData) => new
        (
            gameData.Ranks,
            gameData.Heroes,
            gameData.Cards,
            gameData.RankUps,
            gameData.Rewards
        );
    }
}