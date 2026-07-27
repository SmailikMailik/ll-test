using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Currencies;
using R3;
using VContainer;

namespace LL.User.Core.Wallet
{
    internal sealed class UserWallet : IUserWallet, IDisposable
    {
        private readonly IReadOnlyDictionary<CurrencyId, ReactiveProperty<int>> _balances;

        [Inject]
        internal UserWallet(WalletInitialData initialData)
        {
            if (initialData == null)
                throw new ArgumentNullException(nameof(initialData));

            _balances = initialData.Balances.ToDictionary(
                balance => balance.Id,
                balance => new ReactiveProperty<int>(balance.Amount));
        }

        public Observable<int> ObserveAmount(CurrencyId id) => GetBalance(id);

        public bool CanAdd(CurrencyId id, int amount)
        {
            return TryGetBalance(id, out var balance) &&
                   amount > 0 &&
                   balance.Value <= int.MaxValue - amount;
        }

        public bool TryAdd(CurrencyId id, int amount)
        {
            if (CanAdd(id, amount) is false)
                return false;

            return TryAdd(GetBalance(id), amount);
        }

        public bool TrySpend(CurrencyId id, int amount)
        {
            return TryGetBalance(id, out var balance) && TrySpend(balance, amount);
        }

        public void Dispose()
        {
            foreach (var balance in _balances.Values)
                balance.Dispose();
        }

        private ReactiveProperty<int> GetBalance(CurrencyId id)
        {
            if (TryGetBalance(id, out var balance))
                return balance;

            throw new KeyNotFoundException($"Unknown currency ID: {id}");
        }

        private bool TryGetBalance(CurrencyId id, out ReactiveProperty<int> balance)
        {
            if (id.IsEmpty)
            {
                balance = null;
                return false;
            }

            return _balances.TryGetValue(id, out balance);
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