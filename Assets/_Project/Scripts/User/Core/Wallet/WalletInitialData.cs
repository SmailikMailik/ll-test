using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Currencies;

namespace LL.User.Core.Wallet
{
    internal sealed class WalletInitialData
    {
        internal IReadOnlyList<CurrencyBalance> Balances { get; }

        internal WalletInitialData(IEnumerable<CurrencyBalance> balances)
        {
            var amounts = new Dictionary<CurrencyId, long>();

            if (balances != null)
            {
                foreach (var balance in balances)
                {
                    if (balance == null || balance.Id.IsEmpty)
                        continue;

                    amounts.TryGetValue(balance.Id, out var current);
                    amounts[balance.Id] = Math.Min(
                        current + balance.Amount,
                        int.MaxValue);
                }
            }

            var copy = amounts
                .Select(pair => new CurrencyBalance(
                    pair.Key,
                    (int)pair.Value))
                .ToArray();

            Balances = Array.AsReadOnly(copy);
        }
    }
}