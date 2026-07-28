using System;
using System.Collections.Generic;
using System.Linq;
using LL.Identifiers;
using R3;
using VContainer;

namespace LL.User.Core.Amounts
{
    internal sealed class UserAmounts<TId> : IUserAmounts<TId>, IDisposable
        where TId : struct, IIdentifier
    {
        private readonly IReadOnlyDictionary<TId, ReactiveProperty<int>> _amounts;

        [Inject]
        internal UserAmounts(AmountsInitialData<TId> initialData)
        {
            if (initialData == null)
                throw new ArgumentNullException(nameof(initialData));

            _amounts = initialData.Amounts.ToDictionary(
                amount => amount.Id,
                amount => new ReactiveProperty<int>(amount.Value));
        }

        public Observable<int> ObserveAmount(TId id)
        {
            if (TryGetAmount(id, out var amount))
                return amount;

            throw new KeyNotFoundException($"Unknown {typeof(TId).Name}: {id}");
        }

        public bool CanAdd(TId id, int amount)
        {
            return TryGetAmount(id, out var currentAmount) &&
                   amount > 0 &&
                   currentAmount.Value <= int.MaxValue - amount;
        }

        public bool TryAdd(TId id, int amount)
        {
            if (CanAdd(id, amount) is false)
                return false;

            TryGetAmount(id, out var currentAmount);
            currentAmount.Value += amount;
            return true;
        }

        public bool TrySpend(TId id, int amount)
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

        private bool TryGetAmount(TId id, out ReactiveProperty<int> amount)
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