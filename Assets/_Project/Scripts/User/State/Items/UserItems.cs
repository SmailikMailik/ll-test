using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Items;
using LL.User.Snapshots.Items;
using R3;
using VContainer;

namespace LL.User.State.Items
{
    internal sealed class UserItems : IUserItems, IDisposable
    {
        private readonly IReadOnlyDictionary<ItemId, ReactiveProperty<int>> _amounts;

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
            if (TryGetAmount(id, out var amount))
                return amount;

            throw new KeyNotFoundException($"Unknown item ID: {id}");
        }

        public bool CanAdd(ItemId id, int amount)
        {
            return TryGetAmount(id, out var currentAmount) &&
                   amount > 0 &&
                   currentAmount.Value <= int.MaxValue - amount;
        }

        public bool TryAdd(ItemId id, int amount)
        {
            if (CanAdd(id, amount) is false)
                return false;

            TryGetAmount(id, out var currentAmount);
            currentAmount.Value += amount;
            return true;
        }

        public bool TrySpend(ItemId id, int amount)
        {
            if (amount <= 0 ||
                TryGetAmount(id, out var currentAmount) is false ||
                currentAmount.Value < amount)
            {
                return false;
            }

            currentAmount.Value -= amount;
            return true;
        }

        public void Dispose()
        {
            foreach (var amount in _amounts.Values)
                amount.Dispose();
        }

        private bool TryGetAmount(ItemId id, out ReactiveProperty<int> amount)
        {
            if (id.IsEmpty)
            {
                amount = null;
                return false;
            }

            return _amounts.TryGetValue(id, out amount);
        }
    }
}