using System;
using LL.User.State;
using R3;
using VContainer;
using VContainer.Unity;

namespace LL.User.Persistence
{
    internal sealed class UserSaveCoordinator : IInitializable, IDisposable
    {
        private readonly UserState _state;
        private readonly IUserSaveRepository _repository;

        private IDisposable _subscription;

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
        }

        private void OnStateChanged(Unit _)
        {
            if (_repository.Save(_state.CreateSnapshot()) is false)
                throw new InvalidOperationException("User state could not be saved.");
        }
    }
}