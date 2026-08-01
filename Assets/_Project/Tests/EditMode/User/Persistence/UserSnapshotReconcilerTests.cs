using System;
using System.Linq;
using LL.Game.Heroes;
using LL.Game.Items;
using LL.Game.Ranks;
using LL.Game.RankUp;
using LL.Tests.EditMode.TestData;
using LL.User.Persistence;
using LL.User.Snapshots;
using NUnit.Framework;

namespace LL.Tests.EditMode.User.Persistence
{
    internal sealed class UserSnapshotReconcilerTests
    {
        private static readonly DateTimeOffset _now = DateTimeOffset.FromUnixTimeMilliseconds(1_000_000L);

        private UserSnapshotReconciler _reconciler;

        [SetUp]
        public void SetUp()
        {
            _reconciler = CreateReconciler();
        }

        [Test]
        public void CompatibleSnapshotWithoutChangesIsPreserved()
        {
            var snapshot = UserTestData.CreateSnapshot();

            var result = _reconciler.Reconcile(snapshot);

            Assert.That(result.Status, Is.EqualTo(UserReconciliationStatus.Unchanged));
            Assert.That(result.Snapshot, Is.SameAs(snapshot));
        }

        [Test]
        public void MissingDefaultItemsAreAdded()
        {
            var snapshot = UserTestData.CreateSnapshot(items: new[] { new ItemAmount(ItemIds.Soft, 10) });

            var result = _reconciler.Reconcile(snapshot);

            Assert.That(result.Status, Is.EqualTo(UserReconciliationStatus.Changed));
            Assert.That(result.Snapshot.Items.Amounts, Has.Count.EqualTo(ItemIds.All.Count));
            Assert.That(
                result.Snapshot.Items.Amounts.Single(item => item.Id.Equals(ItemIds.Soft)).Amount,
                Is.EqualTo(10));
        }

        [Test]
        public void ExpiredRankUpAttemptIsClearedUsingProvidedTime()
        {
            var attempt = new UserRankUpAttemptSnapshot(
                new RankId("bronze"),
                new RankUpOptionId("quest"),
                new[]
                {
                    new UserRankUpQuestRequirementSnapshot(
                        new RankUpRequirementId("quest"),
                        0,
                        _now.AddMilliseconds(-1d).ToUnixTimeMilliseconds())
                });
            var snapshot = UserTestData.CreateSnapshot(rankUpAttempts: new[] { attempt });

            var result = _reconciler.Reconcile(snapshot);

            Assert.That(result.Status, Is.EqualTo(UserReconciliationStatus.Changed));
            Assert.That(result.Snapshot.Heroes.TryGetHero(new HeroId("hero"), out var hero), Is.True);
            Assert.That(hero.RankUpAttempts, Is.Empty);
        }

        [Test]
        public void UnknownHeroRankIsReplacedWithDefaultProgress()
        {
            var snapshot = UserTestData.CreateSnapshot(rankId: new RankId("unknown"));

            var result = _reconciler.Reconcile(snapshot);

            Assert.That(result.Status, Is.EqualTo(UserReconciliationStatus.Changed));
            Assert.That(result.Snapshot.Heroes.TryGetHero(new HeroId("hero"), out var hero), Is.True);
            Assert.That(hero.Progress.RankId, Is.EqualTo(new RankId("bronze")));
        }

        [Test]
        public void UnknownHeroIsReplacedWithDefaultSelection()
        {
            var snapshot = UserTestData.CreateSnapshot(heroId: new HeroId("unknown"));

            var result = _reconciler.Reconcile(snapshot);

            Assert.That(result.Status, Is.EqualTo(UserReconciliationStatus.Changed));
            Assert.That(result.Snapshot.HeroSelection.HeroId, Is.EqualTo(new HeroId("hero")));
        }

        private static UserSnapshotReconciler CreateReconciler()
        {
            var gameData = GameTestData.CreateSnapshot();

            return new UserSnapshotReconciler(
                UserTestData.CreateDefaults(gameData),
                gameData.Heroes,
                new RankProgression(gameData.Ranks),
                GameTestData.CreateRankUpCatalog(),
                new ManualTimeProvider(_now));
        }
    }
}