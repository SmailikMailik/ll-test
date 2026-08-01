using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LL.Game.Data.Persistence.Documents;
using LL.Game.Items;
using LL.Game.Ranks;
using LL.Infrastructure.Saving;
using LL.Infrastructure.Saving.Serialization;
using LL.Infrastructure.Saving.Storage;
using LL.User.Persistence;
using LL.User.Snapshots;
using LL.User.State;
using LL.User.State.Heroes;
using LL.User.State.Items;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LL.Tests.EditMode
{
    internal sealed class PersistenceTests
    {
        private static readonly DateTimeOffset _now = DateTimeOffset.FromUnixTimeMilliseconds(1_000_000L);

        [Test]
        public void SerializedRepositoryRoundTripPreservesUser()
        {
            var storage = new MemorySaveStorage();
            var repository = new SerializedUserSaveRepository(new SaveService(new JsonSaveSerializer(), storage));
            var source = TestDataFactory.CreateUserSnapshot(items: new[] { new ItemAmount(ItemIds.Soft, 15) });

            Assert.That(repository.Save(source), Is.True);

            var result = repository.Load();

            Assert.That(result.Status, Is.EqualTo(UserLoadStatus.Loaded));
            Assert.That(UserSaveDocumentMapper.ToDocument(result.Snapshot).Version, Is.EqualTo(2));
            Assert.That(result.Snapshot.Identity.UserId, Is.EqualTo(source.Identity.UserId));
            Assert.That(result.Snapshot.Items.Amounts[0].Amount, Is.EqualTo(15));
        }

        [Test]
        public void ReadOnlyReaderLoadsWithoutWritableStorageContract()
        {
            var serializer = new JsonSaveSerializer();
            var writableStorage = new MemorySaveStorage();
            var service = new SaveService(serializer, writableStorage);
            var document = UserSaveDocumentMapper.ToDocument(TestDataFactory.CreateUserSnapshot());
            Assert.That(service.TrySave("user", document), Is.True);
            IReadOnlySaveStorage readOnlyStorage = writableStorage;
            var reader = new SaveReader(serializer, readOnlyStorage);

            var loaded = reader.TryLoad("user", out LL.User.Persistence.Documents.UserSaveDocument restored);

            Assert.That(loaded, Is.True);
            Assert.That(restored.Identity.UserId, Is.EqualTo("test-user"));
        }

        [Test]
        public void GameDataDocumentRoundTripUsesRequiredCount()
        {
            var serializer = new JsonSaveSerializer();
            var document = new GameDataDocument(
                GameDataDocument.CurrentVersion,
                Array.Empty<RankDocumentEntry>(),
                Array.Empty<CardDocumentEntry>(),
                Array.Empty<HeroDocumentEntry>(),
                Array.Empty<QuestDocumentEntry>(),
                new[]
                {
                    new RankUpDocumentEntry(
                        "hero",
                        "bronze",
                        "reward",
                        new[]
                        {
                            new RankUpOptionDocumentEntry(
                                "quest",
                                new RankUpRequirementDocumentEntry[]
                                {
                                    new QuestRankUpRequirementDocumentEntry("quest", "quest", 20, 10),
                                    new PaymentRankUpRequirementDocumentEntry(
                                        "soft-payment",
                                        new PaymentDocumentEntry("soft", 1))
                                })
                        })
                },
                Array.Empty<RewardDocumentEntry>());

            Assert.That(serializer.TrySerialize(document, out var bytes), Is.True);

            var json = Encoding.UTF8.GetString(bytes);
            Assert.That(json, Does.Contain("\"Type\": \"quest\""));
            Assert.That(json, Does.Contain("\"Type\": \"payment\""));
            Assert.That(json, Does.Contain("\"RequiredCount\": 20"));
            Assert.That(json, Does.Not.Contain("RequiredAmount"));
            Assert.That(json, Does.Not.Contain("$type"));
            Assert.That(serializer.TryDeserialize(bytes, out GameDataDocument restored), Is.True);
            Assert.That(restored.Version, Is.EqualTo(6));
            Assert.That(
                restored.RankUps[0].Options[0].Requirements[0],
                Is.TypeOf<QuestRankUpRequirementDocumentEntry>());
            Assert.That(
                ((QuestRankUpRequirementDocumentEntry)restored.RankUps[0].Options[0].Requirements[0]).RequiredCount,
                Is.EqualTo(20));
        }

        [Test]
        public void SaveCoordinatorDebouncesChangesAndWritesLatestSnapshot()
        {
            var timeProvider = new ManualTimeProvider(_now);
            var gameData = TestDataFactory.CreateGameDataSnapshot();
            var initial = TestDataFactory.CreateUserSnapshot();
            var items = new UserItems(initial.Items);
            var progression = new RankProgression(gameData.Ranks);
            var heroes = new UserHeroes(initial.Heroes, progression, timeProvider);
            var state = new UserState(initial.Identity, initial.HeroSelection, heroes, items);
            var repository = new RecordingUserSaveRepository();
            var coordinator = new UserSaveCoordinator(state, repository, timeProvider);

            try
            {
                state.Initialize();
                coordinator.Initialize();

                Assert.That(items.TryAdd(ItemIds.Soft, 1), Is.True);
                Assert.That(items.TryAdd(ItemIds.Soft, 2), Is.True);
                coordinator.Tick();
                Assert.That(repository.SaveCount, Is.Zero);

                timeProvider.Advance(TimeSpan.FromMilliseconds(500d));
                coordinator.Tick();

                Assert.That(repository.SaveCount, Is.EqualTo(1));
                Assert.That(
                    repository.LastSnapshot.Items.Amounts.Single(item => item.Id.Equals(ItemIds.Soft)).Amount,
                    Is.EqualTo(3));
            }
            finally
            {
                coordinator.Dispose();
                state.Dispose();
                items.Dispose();
                heroes.Dispose();
            }
        }

        [Test]
        public void SaveCoordinatorRetriesFailedWriteWithoutAnotherStateChange()
        {
            var timeProvider = new ManualTimeProvider(_now);
            var gameData = TestDataFactory.CreateGameDataSnapshot();
            var initial = TestDataFactory.CreateUserSnapshot();
            var items = new UserItems(initial.Items);
            var progression = new RankProgression(gameData.Ranks);
            var heroes = new UserHeroes(initial.Heroes, progression, timeProvider);
            var state = new UserState(initial.Identity, initial.HeroSelection, heroes, items);
            var repository = new RecordingUserSaveRepository(1);
            var coordinator = new UserSaveCoordinator(state, repository, timeProvider);

            try
            {
                state.Initialize();
                coordinator.Initialize();
                Assert.That(items.TryAdd(ItemIds.Soft, 1), Is.True);

                timeProvider.Advance(TimeSpan.FromMilliseconds(500d));
                LogAssert.Expect(LogType.Error, "User state could not be saved. Saving will retry automatically.");
                coordinator.Tick();
                Assert.That(repository.SaveCount, Is.EqualTo(1));

                timeProvider.Advance(TimeSpan.FromMilliseconds(500d));
                coordinator.Tick();
                Assert.That(repository.SaveCount, Is.EqualTo(2));
            }
            finally
            {
                coordinator.Dispose();
                state.Dispose();
                items.Dispose();
                heroes.Dispose();
            }
        }

        [Test]
        public void FileStorageAtomicallyReplacesExistingData()
        {
            var directoryPath = Path.Combine(Path.GetTempPath(), $"ll-persistence-tests-{Guid.NewGuid():N}");

            try
            {
                var storage = new FileSaveStorage(directoryPath);

                Assert.That(storage.TryWrite("user", new byte[] { 1 }), Is.True);
                Assert.That(storage.TryWrite("user", new byte[] { 2, 3 }), Is.True);
                Assert.That(storage.TryRead("user", out var bytes), Is.True);
                Assert.That(bytes, Is.EqualTo(new byte[] { 2, 3 }));
                Assert.That(File.Exists(Path.Combine(directoryPath, "user.save.tmp")), Is.False);
            }
            finally
            {
                if (Directory.Exists(directoryPath))
                    Directory.Delete(directoryPath, true);
            }
        }

        private sealed class MemorySaveStorage : ISaveStorage
        {
            private readonly Dictionary<string, byte[]> _entries = new();

            public bool Exists(string key) => _entries.ContainsKey(key);

            public bool TryWrite(string key, byte[] data)
            {
                _entries[key] = (byte[])data.Clone();
                return true;
            }

            public bool TryRead(string key, out byte[] data)
            {
                if (_entries.TryGetValue(key, out var stored) is false)
                {
                    data = null;
                    return false;
                }

                data = (byte[])stored.Clone();
                return true;
            }

            public bool TryDelete(string key) => _entries.Remove(key);
        }

        private sealed class RecordingUserSaveRepository : IUserSaveRepository
        {
            private int _failuresRemaining;

            internal int SaveCount { get; private set; }
            internal UserSnapshot LastSnapshot { get; private set; }

            internal RecordingUserSaveRepository(int failuresRemaining = 0)
            {
                _failuresRemaining = failuresRemaining;
            }

            public UserLoadResult Load() => UserLoadResult.Failed(UserLoadStatus.NotFound);

            public bool Save(UserSnapshot snapshot)
            {
                SaveCount++;

                if (_failuresRemaining > 0)
                {
                    _failuresRemaining--;
                    return false;
                }

                LastSnapshot = snapshot;
                return true;
            }

            public bool Exists() => LastSnapshot is not null;
            public bool Delete() => true;
        }
    }
}