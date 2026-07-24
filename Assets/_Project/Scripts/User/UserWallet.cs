using System;
using R3;

namespace LL.User
{
    internal sealed class UserWallet : IUserWallet, IDisposable
    {
        private readonly ReactiveProperty<int> _softAmount;
        private readonly ReactiveProperty<int> _hardAmount;
        private readonly ReactiveProperty<int> _masterPointAmount;

        internal UserWallet(UserDataSnapshot initialData)
        {
            _softAmount = new ReactiveProperty<int>(initialData.SoftAmount);
            _hardAmount = new ReactiveProperty<int>(initialData.HardAmount);
            _masterPointAmount = new ReactiveProperty<int>(initialData.MasterPointAmount);
        }

        public Observable<int> ObserveAmount(CurrencyType type) => GetBalance(type);

        public bool TryAdd(CurrencyType type, int amount)
        {
            return TryGetBalance(type, out var balance) && TryAdd(balance, amount);
        }

        public bool TrySpend(CurrencyType type, int amount)
        {
            return TryGetBalance(type, out var balance) && TrySpend(balance, amount);
        }

        public void Dispose()
        {
            _softAmount.Dispose();
            _hardAmount.Dispose();
            _masterPointAmount.Dispose();
        }

        private ReactiveProperty<int> GetBalance(CurrencyType type)
        {
            if (TryGetBalance(type, out var balance))
                return balance;

            throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown currency type");
        }

        private bool TryGetBalance(CurrencyType type, out ReactiveProperty<int> balance)
        {
            balance = type switch
            {
                CurrencyType.Soft => _softAmount,
                CurrencyType.Hard => _hardAmount,
                CurrencyType.MasterPoint => _masterPointAmount,
                _ => null
            };

            return balance != null;
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