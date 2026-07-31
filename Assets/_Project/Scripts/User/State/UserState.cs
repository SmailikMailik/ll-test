using System;
using System.Collections.Generic;
using LL.User.Snapshots;
using LL.User.State.Items;
using LL.User.State.Progress;
using LL.User.State.RankUp;
using R3;
using VContainer;
using VContainer.Unity;

namespace LL.User.State
{
    internal sealed class UserState : IUserStateChangeBatch, IInitializable, IDisposable
    {
        internal Observable<Unit> Changed => _changed;

        private readonly UserIdentitySnapshot _identity;
        private readonly IUserItems _items;
        private readonly IUserProgress _progress;
        private readonly IUserRankUpQuest _rankUpQuest;

        private readonly Subject<Unit> _changed = new();
        private readonly List<IDisposable> _subscriptions = new();

        private int _changeBatchDepth;
        private bool _hasPendingChange;

        [Inject]
        internal UserState(
            UserIdentitySnapshot identity,
            IUserItems items,
            IUserProgress progress,
            IUserRankUpQuest rankUpQuest)
        {
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));
            _items = items ?? throw new ArgumentNullException(nameof(items));
            _progress = progress ?? throw new ArgumentNullException(nameof(progress));
            _rankUpQuest = rankUpQuest ?? throw new ArgumentNullException(nameof(rankUpQuest));
        }

        public void Initialize()
        {
            _subscriptions.Add(_items.Changed.Subscribe(OnStateChanged));
            _subscriptions.Add(_progress.Changed.Subscribe(OnStateChanged));
            _subscriptions.Add(_rankUpQuest.Changed.Subscribe(OnStateChanged));
        }

        internal UserSnapshot CreateSnapshot()
        {
            return new UserSnapshot(
                _identity,
                _items.CreateSnapshot(),
                _progress.CreateSnapshot(),
                _rankUpQuest.CreateSnapshot());
        }

        public TResult Execute<TResult>(Func<TResult> mutation)
        {
            if (mutation == null)
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