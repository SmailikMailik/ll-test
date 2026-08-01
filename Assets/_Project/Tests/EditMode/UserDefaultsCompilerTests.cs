using System;
using System.Linq;
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
            Assert.That(snapshot.Progress.RankId.Value, Is.EqualTo("bronze"));
            Assert.That(snapshot.Items.Amounts, Has.Count.EqualTo(ItemIds.All.Count));
        }

        [Test]
        public void MissingRequiredItemIsRejected()
        {
            var gameData = TestDataFactory.CreateGameDataSnapshot();
            var compiler = new UserDefaultsCompiler(
                gameData.Ranks,
                gameData.Cards,
                gameData.RankUps,
                gameData.Rewards);
            var declaration = new UserDefaultsDeclaration(
                "test-user",
                "RU",
                "bronze",
                0,
                ItemIds.All
                    .Where(id => id.Equals(ItemIds.Hard) is false)
                    .Select(id => new UserItemDefaultsDeclaration(id.Value, 0)));

            Assert.Throws<ArgumentException>(() => compiler.Compile(declaration));
        }
    }
}