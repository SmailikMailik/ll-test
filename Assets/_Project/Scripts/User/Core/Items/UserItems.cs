using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Items;
using R3;
using VContainer;

namespace LL.User.Core.Items
{
    internal sealed class UserItems : IUserItems, IDisposable
    {
        private readonly IReadOnlyDictionary<ItemId, ReactiveProperty<int>> _amounts;

        [Inject]
        internal UserItems(ItemsInitialData initialData)
        {
            if (initialData == null)
                throw new ArgumentNullException(nameof(initialData));

            _amounts = initialData.Amounts.ToDictionary(
                item => item.Id,
                item => new ReactiveProperty<int>(item.Amount));
        }

        public Observable<int> ObserveAmount(ItemId id) => GetAmount(id);

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

            return TryAdd(GetAmount(id), amount);
        }

        public bool TrySpend(ItemId id, int amount)
        {
            return TryGetAmount(id, out var currentAmount) && TrySpend(currentAmount, amount);
        }

        public void Dispose()
        {
            foreach (var amount in _amounts.Values)
                amount.Dispose();
        }

        private ReactiveProperty<int> GetAmount(ItemId id)
        {
            if (TryGetAmount(id, out var amount))
                return amount;

            throw new KeyNotFoundException($"Unknown item ID: {id}");
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

        private static bool TryAdd(ReactiveProperty<int> balance, int amount)
        {
            if (amount <= 0 || balance.Value > int.MaxValue - amount)
                return false;

            balance.Value += amount;
            return true;
        }

        private static bool TrySpend(ReactiveProperty<int> balance, int amount)
        {
            if (amount <= 0 || balance.Value < amount)
                return false;

            balance.Value -= amount;
            return true;
        }
    }
}