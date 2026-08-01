using System;
using LL.Game.Heroes;
using LL.Game.Ranks;
using LL.Game.RankUp;
using LL.User.Snapshots;
using LL.User.State.Heroes;
using NUnit.Framework;

namespace LL.Tests.EditMode
{
    internal sealed class UserHeroesTests
    {
        private static readonly DateTimeOffset _now = DateTimeOffset.FromUnixTimeMilliseconds(1_000_000L);

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
            var heroes = new UserHeroes(
                snapshot,
                new RankProgression(TestDataFactory.CreateGameDataSnapshot().Ranks),
                timeProvider);

            Assert.That(heroes.TryAddExperience(soldierId, 25), Is.True);
            Assert.That(heroes.TryStartQuest(
                soldierId,
                rankId,
                optionId,
                requirementId,
                TimeSpan.FromHours(22d)), Is.True);
            Assert.That(heroes.TryAddQuestProgress(soldierId, rankId, optionId, requirementId, 5), Is.True);

            timeProvider.Advance(TimeSpan.FromHours(1d));

            Assert.That(heroes.TryAddExperience(captainId, 40), Is.True);
            Assert.That(heroes.TryStartQuest(
                captainId,
                rankId,
                optionId,
                requirementId,
                TimeSpan.FromHours(23d)), Is.True);
            Assert.That(heroes.TryAddQuestProgress(captainId, rankId, optionId, requirementId, 10), Is.True);

            Assert.That(heroes.TryGetProgress(soldierId, out var soldierProgress), Is.True);
            Assert.That(heroes.TryGetProgress(captainId, out var captainProgress), Is.True);
            Assert.That(soldierProgress.Experience, Is.EqualTo(25));
            Assert.That(captainProgress.Experience, Is.EqualTo(40));
            Assert.That(heroes.TryGetQuest(soldierId, rankId, optionId, requirementId, out var soldierQuest), Is.True);
            Assert.That(heroes.TryGetQuest(captainId, rankId, optionId, requirementId, out var captainQuest), Is.True);
            Assert.That(soldierQuest.CurrentCount, Is.EqualTo(5));
            Assert.That(captainQuest.CurrentCount, Is.EqualTo(10));
            Assert.That(heroes.GetRemainingTime(soldierQuest), Is.EqualTo(TimeSpan.FromHours(21d)));
            Assert.That(heroes.GetRemainingTime(captainQuest), Is.EqualTo(TimeSpan.FromHours(23d)));
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