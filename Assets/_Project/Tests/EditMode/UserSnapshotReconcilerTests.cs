using System;
using System.Linq;
using LL.Game.Items;
using LL.Game.Quests;
using LL.Game.Ranks;
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
        public void ExpiredRankUpQuestIsClearedUsingProvidedTime()
        {
            var quest = new UserRankUpQuestSnapshot(
                new QuestId("quest"),
                _now.AddMilliseconds(-1d).ToUnixTimeMilliseconds(),
                false);
            var snapshot = TestDataFactory.CreateUserSnapshot(rankUpQuest: quest);
            var reconciler = CreateReconciler();

            var result = reconciler.Reconcile(snapshot);

            Assert.That(result.Status, Is.EqualTo(UserReconciliationStatus.Changed));
            Assert.That(result.Snapshot.RankUpQuest, Is.SameAs(UserRankUpQuestSnapshot.Empty));
        }

        [Test]
        public void UnknownRankReturnsExplicitIncompatibleResult()
        {
            var snapshot = TestDataFactory.CreateUserSnapshot(rankId: new RankId("unknown"));
            var reconciler = CreateReconciler();

            var result = reconciler.Reconcile(snapshot);

            Assert.That(result.Status, Is.EqualTo(UserReconciliationStatus.Incompatible));
            Assert.That(result.Snapshot, Is.Null);
        }

        private static UserSnapshotReconciler CreateReconciler()
        {
            var gameData = TestDataFactory.CreateGameDataSnapshot();

            return new UserSnapshotReconciler(
                TestDataFactory.CreateUserDefaults(gameData),
                new RankProgression(gameData.Ranks),
                TestDataFactory.CreateRankUpCatalog(),
                new ManualTimeProvider(_now));
        }
    }
}