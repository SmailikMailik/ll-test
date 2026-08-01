using System;
using LL.Game.Data;
using LL.Game.Ranks;
using NUnit.Framework;

namespace LL.Tests.EditMode
{
    internal sealed class GameDataCompilerTests
    {
        [Test]
        public void ValidDeclarationBuildsConsistentSnapshot()
        {
            var snapshot = new GameDataCompiler().Compile(TestDataFactory.CreateGameDataDeclaration());
            var progress = new RankProgression(snapshot.Ranks).GetProgress(new RankId("bronze"), 0);

            Assert.That(snapshot.Ranks.Ranks, Has.Count.EqualTo(2));
            Assert.That(snapshot.Ranks.GetRank(new RankId("silver")).Number, Is.EqualTo(2));
            Assert.That(snapshot.RankUps.Definitions, Has.Count.EqualTo(1));
            Assert.That(snapshot.Rewards.Rewards, Has.Count.EqualTo(1));
            Assert.That(progress.RankNumber, Is.EqualTo(1));
            Assert.That(progress.ExperienceRequiredForRankUp, Is.EqualTo(100));
        }

        [Test]
        public void MissingRankUpRewardIsRejected()
        {
            var declaration = TestDataFactory.CreateGameDataDeclaration("missing-reward");

            Assert.Throws<ArgumentException>(() => new GameDataCompiler().Compile(declaration));
        }
    }
}