using System;
using LL.User.State;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace LL.User.Persistence
{
    internal sealed class UserSaveCoordinator : IInitializable, ITickable, IDisposable
    {
        private static readonly TimeSpan _saveDelay = TimeSpan.FromMilliseconds(500d);

        private readonly UserState _state;
        private readonly IUserSaveRepository _repository;
        private readonly TimeProvider _timeProvider;

        private IDisposable _subscription;
        private DateTimeOffset _saveDueAt;
        private bool _isDirty;
        private bool _saveFailureReported;

        [Inject]
        internal UserSaveCoordinator(
            UserState state,
            IUserSaveRepository repository,
            TimeProvider timeProvider)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        }

        public void Initialize()
        {
            _subscription = _state.Changed.Subscribe(OnStateChanged);
        }

        public void Dispose()
        {
            _subscription?.Dispose();
            _subscription = null;

            if (_isDirty)
                TrySave();
        }

        public void Tick()
        {
            if (_isDirty && _timeProvider.GetUtcNow() >= _saveDueAt)
                TrySave();
        }

        private void OnStateChanged(Unit _)
        {
            _isDirty = true;
            _saveDueAt = _timeProvider.GetUtcNow().Add(_saveDelay);
        }

        private void TrySave()
        {
            if (_repository.Save(_state.CreateSnapshot()))
            {
                _isDirty = false;
                _saveFailureReported = false;
                return;
            }

            if (_saveFailureReported)
            {
                _saveDueAt = _timeProvider.GetUtcNow().Add(_saveDelay);
                return;
            }

            _saveFailureReported = true;
            _saveDueAt = _timeProvider.GetUtcNow().Add(_saveDelay);
            Debug.LogError("User state could not be saved. Saving will retry automatically.");
        }
    }
}