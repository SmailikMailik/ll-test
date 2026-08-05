using System;
using System.Linq;
using LL.Game.Items;
using LL.Game.Ranks;
using LL.Infrastructure.Saving;
using LL.Infrastructure.Saving.Serialization;
using LL.Tests.EditMode.Infrastructure.Saving.Storage;
using LL.Tests.EditMode.TestData;
using LL.User.Persistence;
using LL.User.Persistence.Documents;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LL.Tests.EditMode.User.Persistence
{
    internal sealed class UserSessionLoaderTests
    {
        private const int UnsupportedVersion = 2;
        private const int SavedSoftAmount = 15;

        [Test]
        public void UnsupportedVersionIsResetToDefaultsAndSavedAsCurrentVersion()
        {
            var storage = new MemorySaveStorage();
            var saveService = new SaveService(new JsonSaveSerializer(), storage);
            SaveUnsupportedUser(saveService);
            var loader = CreateLoader(saveService, out var repository);

            LogAssert.Expect(
                LogType.Warning,
                $"Unsupported user document version {UnsupportedVersion}. Resetting user data.");
            var loadedSnapshot = loader.Load();
            var persistedResult = repository.Load();
            var loadedSoftAmount = loadedSnapshot.Items.Amounts
                .Single(item => item.Id.Equals(ItemIds.Soft)).Amount;
            var persistedDocument = UserSaveDocumentMapper.ToDocument(persistedResult.Snapshot);

            Assert.That(loadedSoftAmount, Is.Zero);
            Assert.That(persistedResult.Status, Is.EqualTo(UserLoadStatus.Loaded));
            Assert.That(persistedDocument.Version, Is.EqualTo(UserSaveDocument.CurrentVersion));
        }

        private static void SaveUnsupportedUser(ISaveService saveService)
        {
            var previousSnapshot = UserTestData.CreateSnapshot(
                items: new[] { new ItemAmount(ItemIds.Soft, SavedSoftAmount) });
            var previous = UserSaveDocumentMapper.ToDocument(previousSnapshot);
            var unsupported = new UserSaveDocument(
                UnsupportedVersion,
                previous.Identity,
                previous.HeroSelection,
                previous.Heroes,
                previous.Items);

            Assert.That(saveService.TrySave(SerializedUserSaveRepository.SaveKey, unsupported), Is.True);
        }

        private static UserSessionLoader CreateLoader(
            ISaveService saveService,
            out SerializedUserSaveRepository repository)
        {
            var gameData = GameTestData.CreateSnapshot();
            var defaults = UserTestData.CreateDefaults(gameData);
            repository = new SerializedUserSaveRepository(saveService);
            var reconciler = new UserSnapshotReconciler(
                defaults,
                gameData.Heroes,
                new RankProgression(gameData.Ranks),
                gameData.RankUps,
                TimeProvider.System);

            return new UserSessionLoader(defaults, repository, reconciler);
        }
    }
}