using System;
using LL.Game.Data;
using LL.Game.Ranks;
using LL.Game.RankUp;
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
            Assert.That(
                snapshot.RankUps.Definitions[0].TryGetOption(new RankUpOptionId("free"), out var freeOption),
                Is.True);
            Assert.That(freeOption.Requirements, Is.Empty);
        }

        [Test]
        public void MissingRankUpRewardIsRejected()
        {
            var declaration = TestDataFactory.CreateGameDataDeclaration("missing-reward");

            Assert.Throws<ArgumentException>(() => new GameDataCompiler().Compile(declaration));
        }

        [Test]
        public void RankUpOptionMayHaveNoRequirements()
        {
            var option = new RankUpOptionDefinition(
                new RankUpOptionId("free"),
                Array.Empty<RankUpRequirementDefinition>());

            Assert.That(option.Requirements, Is.Empty);
        }
    }
}