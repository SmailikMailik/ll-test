using System;
using LL.Game.Ranks;
using LL.Tests.EditMode.TestData;
using LL.User.Persistence;
using LL.User.Snapshots;
using LL.User.State;
using LL.User.State.Heroes;
using LL.User.State.Items;

namespace LL.Tests.EditMode.User.Persistence
{
    internal sealed class UserSaveCoordinatorTestContext : IDisposable
    {
        internal ManualTimeProvider TimeProvider { get; }
        internal UserItems Items { get; }
        internal RecordingUserSaveRepository Repository { get; }
        internal UserSaveCoordinator Coordinator { get; }

        private readonly UserState _state;
        private readonly UserHeroes _heroes;

        internal UserSaveCoordinatorTestContext(DateTimeOffset now, int failuresBeforeSuccess = 0)
        {
            TimeProvider = new ManualTimeProvider(now);
            var gameData = GameTestData.CreateSnapshot();
            var initial = UserTestData.CreateSnapshot();
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

    internal sealed class RecordingUserSaveRepository : IUserSaveRepository
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