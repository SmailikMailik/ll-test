using System;
using System.Collections.Generic;
using LL.User.Snapshots;
using LL.User.State.Heroes;
using LL.User.State.Items;
using R3;
using VContainer;
using VContainer.Unity;

namespace LL.User.State
{
    internal sealed class UserState : IUserStateChangeBatch, IInitializable, IDisposable
    {
        internal Observable<Unit> Changed => _changed;

        private readonly UserIdentitySnapshot _identity;
        private readonly UserHeroSelectionSnapshot _heroSelection;
        private readonly UserHeroes _heroes;
        private readonly IUserItems _items;

        private readonly Subject<Unit> _changed = new();
        private readonly List<IDisposable> _subscriptions = new();

        private int _changeBatchDepth;
        private bool _hasPendingChange;

        [Inject]
        internal UserState(
            UserIdentitySnapshot identity,
            UserHeroSelectionSnapshot heroSelection,
            UserHeroes heroes,
            IUserItems items)
        {
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));
            _heroSelection = heroSelection ?? throw new ArgumentNullException(nameof(heroSelection));
            _heroes = heroes ?? throw new ArgumentNullException(nameof(heroes));
            _items = items ?? throw new ArgumentNullException(nameof(items));
        }

        public void Initialize()
        {
            _subscriptions.Add(_items.Changed.Subscribe(OnStateChanged));
            _subscriptions.Add(_heroes.Changed.Subscribe(OnStateChanged));
        }

        internal UserSnapshot CreateSnapshot()
        {
            return new UserSnapshot(
                _identity,
                _heroSelection,
                _heroes.CreateSnapshot(),
                _items.CreateSnapshot());
        }

        public TResult Execute<TResult>(Func<TResult> mutation)
        {
            if (mutation is null)
                throw new ArgumentNullException(nameof(mutation));

            _changeBatchDepth++;

            try
            {
                return mutation();
            }
            finally
            {
                _changeBatchDepth--;

                if (_changeBatchDepth == 0 && _hasPendingChange)
                {
                    _hasPendingChange = false;
                    _changed.OnNext(Unit.Default);
                }
            }
        }

        public void Dispose()
        {
            foreach (var subscription in _subscriptions)
                subscription.Dispose();

            _subscriptions.Clear();
            _changed.Dispose();
        }

        private void OnStateChanged(Unit _)
        {
            if (_changeBatchDepth > 0)
            {
                _hasPendingChange = true;
                return;
            }

            _changed.OnNext(Unit.Default);
        }
    }
}