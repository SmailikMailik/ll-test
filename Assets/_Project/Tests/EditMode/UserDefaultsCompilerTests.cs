using System;
using System.Linq;
using LL.Game.Heroes;
using LL.Game.Items;
using LL.User.Defaults;
using LL.User.Defaults.Declarations;
using NUnit.Framework;

namespace LL.Tests.EditMode
{
    internal sealed class UserDefaultsCompilerTests
    {
        [Test]
        public void ValidDeclarationBuildsDefaults()
        {
            var gameData = TestDataFactory.CreateGameDataSnapshot();
            var defaults = TestDataFactory.CreateUserDefaults(gameData);
            var snapshot = defaults.CreateUserSnapshot();

            Assert.That(snapshot.Identity.UserId, Is.EqualTo("test-user"));
            Assert.That(snapshot.HeroSelection.HeroId, Is.EqualTo(new HeroId("hero")));
            Assert.That(snapshot.Progress.RankId.Value, Is.EqualTo("bronze"));
            Assert.That(snapshot.Items.Amounts, Has.Count.EqualTo(ItemIds.All.Count));
        }

        [Test]
        public void MissingRequiredItemIsRejected()
        {
            var gameData = TestDataFactory.CreateGameDataSnapshot();
            var compiler = new UserDefaultsCompiler(
                gameData.Ranks,
                gameData.Heroes,
                gameData.Cards,
                gameData.RankUps,
                gameData.Rewards);
            var declaration = new UserDefaultsDeclaration(
                new UserIdentityDefaultDeclaration("test-user", "RU"),
                new UserHeroSelectionDefaultDeclaration("hero"),
                new UserProgressDefaultDeclaration("bronze", 0),
                ItemIds.All
                    .Where(id => id.Equals(ItemIds.Hard) is false)
                    .Select(id => new UserItemDefaultDeclaration(id.Value, 0)));

            Assert.Throws<ArgumentException>(() => compiler.Compile(declaration));
        }

        [Test]
        public void UnknownDefaultHeroIsRejected()
        {
            var gameData = TestDataFactory.CreateGameDataSnapshot();
            var compiler = new UserDefaultsCompiler(
                gameData.Ranks,
                gameData.Heroes,
                gameData.Cards,
                gameData.RankUps,
                gameData.Rewards);
            var declaration = new UserDefaultsDeclaration(
                new UserIdentityDefaultDeclaration("test-user", "RU"),
                new UserHeroSelectionDefaultDeclaration("unknown"),
                new UserProgressDefaultDeclaration("bronze", 0),
                ItemIds.All.Select(id => new UserItemDefaultDeclaration(id.Value, 0)));

            Assert.Throws<ArgumentException>(() => compiler.Compile(declaration));
        }
    }
}