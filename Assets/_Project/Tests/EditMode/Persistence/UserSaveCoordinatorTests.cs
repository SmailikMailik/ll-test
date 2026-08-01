using System;
using System.Linq;
using LL.Game.Items;
using LL.Game.Ranks;
using LL.Tests.EditMode;
using LL.User.Persistence;
using LL.User.Snapshots;
using LL.User.State;
using LL.User.State.Heroes;
using LL.User.State.Items;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LL.Tests.EditMode.Persistence
{
    internal sealed class UserSaveCoordinatorTests
    {
        private static readonly DateTimeOffset _now = DateTimeOffset.FromUnixTimeMilliseconds(1_000_000L);

        [Test]
        public void DebouncesChangesAndWritesLatestSnapshot()
        {
            var context = new SaveCoordinatorContext(_now);

            try
            {
                Assert.That(context.Items.TryAdd(ItemIds.Soft, 1), Is.True);
                Assert.That(context.Items.TryAdd(ItemIds.Soft, 2), Is.True);
                context.Coordinator.Tick();
                Assert.That(context.Repository.SaveCount, Is.Zero);

                context.TimeProvider.Advance(TimeSpan.FromMilliseconds(500d));
                context.Coordinator.Tick();

                Assert.That(context.Repository.SaveCount, Is.EqualTo(1));
                Assert.That(
                    context.Repository.LastSnapshot.Items.Amounts
                        .Single(item => item.Id.Equals(ItemIds.Soft)).Amount,
                    Is.EqualTo(3));
            }
            finally
            {
                context.Dispose();
            }
        }

        [Test]
        public void RetriesFailedWriteWithoutAnotherStateChange()
        {
            var context = new SaveCoordinatorContext(_now, failuresBeforeSuccess: 1);

            try
            {
                Assert.That(context.Items.TryAdd(ItemIds.Soft, 1), Is.True);

                context.TimeProvider.Advance(TimeSpan.FromMilliseconds(500d));
                LogAssert.Expect(LogType.Error, "User state could not be saved. Saving will retry automatically.");
                context.Coordinator.Tick();
                Assert.That(context.Repository.SaveCount, Is.EqualTo(1));

                context.TimeProvider.Advance(TimeSpan.FromMilliseconds(500d));
                context.Coordinator.Tick();
                Assert.That(context.Repository.SaveCount, Is.EqualTo(2));
            }
            finally
            {
                context.Dispose();
            }
        }

        private sealed class SaveCoordinatorContext : IDisposable
        {
            internal ManualTimeProvider TimeProvider { get; }
            internal UserItems Items { get; }
            internal RecordingUserSaveRepository Repository { get; }
            internal UserSaveCoordinator Coordinator { get; }

            private readonly UserState _state;
            private readonly UserHeroes _heroes;

            internal SaveCoordinatorContext(DateTimeOffset now, int failuresBeforeSuccess = 0)
            {
                TimeProvider = new ManualTimeProvider(now);
                var gameData = TestDataFactory.CreateGameDataSnapshot();
                var initial = TestDataFactory.CreateUserSnapshot();
                Items = new UserItems(initial.Items);
                _heroes = new UserHeroes(
                    initial.Heroes,
                    new RankProgression(gameData.Ranks),
                    TimeProvider);
                _state = new UserState(initial.Identity, initial.HeroSelection, _heroes, Items);
                Repository = new RecordingUserSaveRepository(failuresBeforeSuccess);
                Coordinator = new UserSaveCoordinator(_state, Repository, TimeProvider);
                _state.Initialize();
                Coordinator.Initialize();
            }

            public void Dispose()
            {
                Coordinator.Dispose();
                _state.Dispose();
                Items.Dispose();
                _heroes.Dispose();
            }
        }

        private sealed class RecordingUserSaveRepository : IUserSaveRepository
        {
            private int _failuresRemaining;

            internal int SaveCount { get; private set; }
            internal UserSnapshot LastSnapshot { get; private set; }

            internal RecordingUserSaveRepository(int failuresBeforeSuccess)
            {
                _failuresRemaining = failuresBeforeSuccess;
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