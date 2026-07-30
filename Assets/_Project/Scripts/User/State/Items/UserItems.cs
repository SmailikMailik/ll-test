using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Items;
using LL.User.Snapshots;
using R3;
using VContainer;

namespace LL.User.State.Items
{
    internal sealed class UserItems : IUserItems, IDisposable
    {
        public Observable<Unit> Changed => _changed;

        private readonly IReadOnlyDictionary<ItemId, ReactiveProperty<int>> _amounts;
        private readonly Subject<Unit> _changed = new();

        [Inject]
        internal UserItems(UserItemsSnapshot snapshot)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));

            _amounts = snapshot.Amounts.ToDictionary(
                amount => amount.Id,
                amount => new ReactiveProperty<int>(amount.Amount));
        }

        public Observable<int> ObserveAmount(ItemId id)
        {
            if (TryGetAmount(id, out var amount) is false)
                throw new KeyNotFoundException($"Unknown item ID: {id}");

            return amount;
        }

        public UserItemsSnapshot CreateSnapshot()
        {
            return new UserItemsSnapshot(_amounts.Select(pair => new ItemAmount(pair.Key, pair.Value.Value)));
        }

        public bool CanAdd(ItemId id, int amount)
        {
            if (amount <= 0)
                return false;

            if (TryGetAmount(id, out var currentAmount) is false)
                return false;

            return currentAmount.Value <= int.MaxValue - amount;
        }

        public bool CanSpend(ItemId id, int amount)
        {
            if (amount <= 0)
                return false;

            if (TryGetAmount(id, out var currentAmount) is false)
                return false;

            return currentAmount.Value >= amount;
        }

        public bool TryAdd(ItemId id, int amount)
        {
            if (CanAdd(id, amount) is false)
                return false;

            TryGetAmount(id, out var currentAmount);
            currentAmount.Value += amount;
            _changed.OnNext(Unit.Default);
            return true;
        }

        public bool TrySpend(ItemId id, int amount)
        {
            if (CanSpend(id, amount) is false)
                return false;

            TryGetAmount(id, out var currentAmount);
            currentAmount.Value -= amount;
            _changed.OnNext(Unit.Default);
            return true;
        }

        public void Dispose()
        {
            foreach (var amount in _amounts.Values)
                amount.Dispose();

            _changed.Dispose();
        }

        private bool TryGetAmount(ItemId id, out ReactiveProperty<int> amount)
        {
            if (string.IsNullOrWhiteSpace(id.Value))
            {
                amount = null;
                return false;
            }

            return _amounts.TryGetValue(id, out amount);
        }
    }
}