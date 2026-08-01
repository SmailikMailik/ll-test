using System;
using System.Linq;
using LL.Game.Items;
using LL.Game.Ranks;
using LL.Infrastructure.Saving;
using LL.Infrastructure.Saving.Serialization;
using LL.Tests.EditMode;
using LL.User.Persistence;
using LL.User.Persistence.Documents;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LL.Tests.EditMode.Persistence
{
    internal sealed class UserSessionLoaderTests
    {
        private static readonly DateTimeOffset _now = DateTimeOffset.FromUnixTimeMilliseconds(1_000_000L);

        [Test]
        public void UnsupportedVersionIsResetToDefaultsAndSavedAsCurrentVersion()
        {
            var storage = new MemorySaveStorage();
            var saveService = new SaveService(new JsonSaveSerializer(), storage);
            SaveUnsupportedUser(saveService);

            var gameData = TestDataFactory.CreateGameDataSnapshot();
            var defaults = TestDataFactory.CreateUserDefaults(gameData);
            var repository = new SerializedUserSaveRepository(saveService);
            var reconciler = new UserSnapshotReconciler(
                defaults,
                gameData.Heroes,
                new RankProgression(gameData.Ranks),
                gameData.RankUps,
                new ManualTimeProvider(_now));
            var loader = new UserSessionLoader(defaults, repository, reconciler);

            LogAssert.Expect(LogType.Warning, "Unsupported user document version 2. Resetting user data.");
            var snapshot = loader.Load();
            var reload = repository.Load();

            Assert.That(
                snapshot.Items.Amounts.Single(item => item.Id.Equals(ItemIds.Soft)).Amount,
                Is.Zero);
            Assert.That(reload.Status, Is.EqualTo(UserLoadStatus.Loaded));
            Assert.That(UserSaveDocumentMapper.ToDocument(reload.Snapshot).Version, Is.EqualTo(1));
        }

        private static void SaveUnsupportedUser(ISaveService saveService)
        {
            var previous = UserSaveDocumentMapper.ToDocument(
                TestDataFactory.CreateUserSnapshot(items: new[] { new ItemAmount(ItemIds.Soft, 15) }));
            var unsupported = new UserSaveDocument(
                2,
                previous.Identity,
                previous.HeroSelection,
                previous.Heroes,
                previous.Items);

            Assert.That(saveService.TrySave(SerializedUserSaveRepository.SaveKey, unsupported), Is.True);
        }
    }
}