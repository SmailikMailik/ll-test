using System;
using System.Linq;
using LL.Game.Heroes;
using LL.Game.Items;
using LL.Game.Ranks;
using LL.Game.RankUp;
using LL.User.Persistence;
using LL.User.Snapshots;
using NUnit.Framework;

namespace LL.Tests.EditMode
{
    internal sealed class UserSnapshotReconcilerTests
    {
        private static readonly DateTimeOffset _now = DateTimeOffset.FromUnixTimeMilliseconds(1_000_000L);

        [Test]
        public void CompatibleSnapshotWithoutChangesIsPreserved()
        {
            var snapshot = TestDataFactory.CreateUserSnapshot();
            var reconciler = CreateReconciler();

            var result = reconciler.Reconcile(snapshot);

            Assert.That(result.Status, Is.EqualTo(UserReconciliationStatus.Unchanged));
            Assert.That(result.Snapshot, Is.SameAs(snapshot));
        }

        [Test]
        public void MissingDefaultItemsAreAdded()
        {
            var snapshot = TestDataFactory.CreateUserSnapshot(items: new[] { new ItemAmount(ItemIds.Soft, 10) });
            var reconciler = CreateReconciler();

            var result = reconciler.Reconcile(snapshot);

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
            var snapshot = TestDataFactory.CreateUserSnapshot(rankUpAttempts: new[] { attempt });
            var reconciler = CreateReconciler();

            var result = reconciler.Reconcile(snapshot);

            Assert.That(result.Status, Is.EqualTo(UserReconciliationStatus.Changed));
            Assert.That(result.Snapshot.Heroes.TryGetHero(new HeroId("hero"), out var hero), Is.True);
            Assert.That(hero.RankUpAttempts, Is.Empty);
        }

        [Test]
        public void UnknownHeroRankIsReplacedWithDefaultProgress()
        {
            var snapshot = TestDataFactory.CreateUserSnapshot(rankId: new RankId("unknown"));
            var reconciler = CreateReconciler();

            var result = reconciler.Reconcile(snapshot);

            Assert.That(result.Status, Is.EqualTo(UserReconciliationStatus.Changed));
            Assert.That(result.Snapshot.Heroes.TryGetHero(new HeroId("hero"), out var hero), Is.True);
            Assert.That(hero.Progress.RankId, Is.EqualTo(new RankId("bronze")));
        }

        [Test]
        public void UnknownHeroIsReplacedWithDefaultSelection()
        {
            var snapshot = TestDataFactory.CreateUserSnapshot(heroId: new HeroId("unknown"));
            var reconciler = CreateReconciler();

            var result = reconciler.Reconcile(snapshot);

            Assert.That(result.Status, Is.EqualTo(UserReconciliationStatus.Changed));
            Assert.That(result.Snapshot.HeroSelection.HeroId, Is.EqualTo(new HeroId("hero")));
        }

        private static UserSnapshotReconciler CreateReconciler()
        {
            var gameData = TestDataFactory.CreateGameDataSnapshot();

            return new UserSnapshotReconciler(
                TestDataFactory.CreateUserDefaults(gameData),
                gameData.Heroes,
                new RankProgression(gameData.Ranks),
                TestDataFactory.CreateRankUpCatalog(),
                new ManualTimeProvider(_now));
        }
    }
}