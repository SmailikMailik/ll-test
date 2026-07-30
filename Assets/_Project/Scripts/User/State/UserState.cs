using System;
using System.Collections.Generic;
using LL.User.Snapshots;
using LL.User.State.Items;
using LL.User.State.Progress;
using LL.User.State.Promotions;
using R3;
using VContainer;
using VContainer.Unity;

namespace LL.User.State
{
    internal sealed class UserState : IInitializable, IDisposable
    {
        internal Observable<Unit> Changed => _changed;

        private readonly UserIdentitySnapshot _identity;
        private readonly IUserItems _items;
        private readonly IUserProgress _progress;
        private readonly IUserPromotionQuest _promotionQuest;
        private readonly Subject<Unit> _changed = new();
        private readonly List<IDisposable> _subscriptions = new();

        [Inject]
        internal UserState(
            UserIdentitySnapshot identity,
            IUserItems items,
            IUserProgress progress,
            IUserPromotionQuest promotionQuest)
        {
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));
            _items = items ?? throw new ArgumentNullException(nameof(items));
            _progress = progress ?? throw new ArgumentNullException(nameof(progress));
            _promotionQuest = promotionQuest ?? throw new ArgumentNullException(nameof(promotionQuest));
        }

        public void Initialize()
        {
            _subscriptions.Add(_items.Changed.Subscribe(OnStateChanged));
            _subscriptions.Add(_progress.Changed.Subscribe(OnStateChanged));
            _subscriptions.Add(_promotionQuest.Changed.Subscribe(OnStateChanged));
        }

        internal UserSnapshot CreateSnapshot()
        {
            return new UserSnapshot(
                _identity,
                _items.CreateSnapshot(),
                _progress.CreateSnapshot(),
                _promotionQuest.CreateSnapshot());
        }

        public void Dispose()
        {
            foreach (var subscription in _subscriptions)
                subscription.Dispose();

            _subscriptions.Clear();
            _changed.Dispose();
        }

        private void OnStateChanged(Unit _) => _changed.OnNext(Unit.Default);
    }
}