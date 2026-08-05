using System;
using LL.Game.Heroes;
using LL.Game.Ranks;
using LL.Game.RankUp;
using LL.Tests.EditMode.TestData;
using LL.User.Snapshots;
using LL.User.State.Heroes;
using NUnit.Framework;

namespace LL.Tests.EditMode.User.State.Heroes
{
    internal sealed class UserHeroesTests
    {
        private static readonly DateTimeOffset _now = DateTimeOffset.FromUnixTimeMilliseconds(1_000_000L);

        private UserHeroes _heroes;

        [TearDown]
        public void TearDown()
        {
            _heroes?.Dispose();
            _heroes = null;
        }

        [Test]
        public void ExperienceAndRankUpAttemptsAreIndependentForEachHero()
        {
            var soldierId = new HeroId("soldier");
            var captainId = new HeroId("captain");
            var rankId = new RankId("bronze");
            var optionId = new RankUpOptionId("quest");
            var requirementId = new RankUpRequirementId("headshots");
            var snapshot = new UserHeroesSnapshot(
                new[]
                {
                    CreateHero(soldierId, rankId),
                    CreateHero(captainId, rankId)
                });
            var timeProvider = new ManualTimeProvider(_now);
            _heroes = new UserHeroes(
                snapshot,
                new RankProgression(GameTestData.CreateSnapshot().Ranks),
                timeProvider);

            Assert.That(_heroes.TryAddExperience(soldierId, 25), Is.True);
            Assert.That(_heroes.TryStartQuest(
                soldierId,
                rankId,
                optionId,
                requirementId,
                TimeSpan.FromHours(22d)), Is.True);
            Assert.That(_heroes.TryAddQuestProgress(soldierId, rankId, optionId, requirementId, 5), Is.True);

            timeProvider.Advance(TimeSpan.FromHours(1d));

            Assert.That(_heroes.TryAddExperience(captainId, 40), Is.True);
            Assert.That(_heroes.TryStartQuest(
                captainId,
                rankId,
                optionId,
                requirementId,
                TimeSpan.FromHours(23d)), Is.True);
            Assert.That(_heroes.TryAddQuestProgress(captainId, rankId, optionId, requirementId, 10), Is.True);

            Assert.That(_heroes.TryGetProgress(soldierId, out var soldierProgress), Is.True);
            Assert.That(_heroes.TryGetProgress(captainId, out var captainProgress), Is.True);
            Assert.That(soldierProgress.Experience, Is.EqualTo(25));
            Assert.That(captainProgress.Experience, Is.EqualTo(40));
            Assert.That(_heroes.TryGetQuest(soldierId, rankId, optionId, requirementId, out var soldierQuest), Is.True);
            Assert.That(_heroes.TryGetQuest(captainId, rankId, optionId, requirementId, out var captainQuest), Is.True);
            Assert.That(soldierQuest.CurrentCount, Is.EqualTo(5));
            Assert.That(captainQuest.CurrentCount, Is.EqualTo(10));
            Assert.That(_heroes.GetRemainingTime(soldierQuest), Is.EqualTo(TimeSpan.FromHours(21d)));
            Assert.That(_heroes.GetRemainingTime(captainQuest), Is.EqualTo(TimeSpan.FromHours(23d)));
        }

        private static UserHeroSnapshot CreateHero(HeroId heroId, RankId rankId)
        {
            return new UserHeroSnapshot(
                heroId,
                new UserProgressSnapshot(rankId, 0),
                Array.Empty<UserRankUpAttemptSnapshot>());
        }
    }
}