using System;
using LL.User.State;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace LL.User.Persistence
{
    internal sealed class UserSaveCoordinator : IInitializable, IDisposable
    {
        private readonly UserState _state;
        private readonly IUserSaveRepository _repository;

        private IDisposable _subscription;
        private bool _isDirty;
        private bool _saveFailureReported;

        [Inject]
        internal UserSaveCoordinator(UserState state, IUserSaveRepository repository)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
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

        private void OnStateChanged(Unit _)
        {
            _isDirty = true;
            TrySave();
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
                return;

            _saveFailureReported = true;
            Debug.LogError("User state could not be saved. The next state change will retry.");
        }
    }
}