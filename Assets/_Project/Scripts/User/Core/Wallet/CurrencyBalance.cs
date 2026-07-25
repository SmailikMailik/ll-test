using System;
using LL.Game.Currencies;

namespace LL.User.Core.Wallet
{
    internal sealed class CurrencyBalance
    {
        internal CurrencyId Id { get; }
        internal int Amount { get; }

        internal CurrencyBalance(CurrencyId id, int amount)
        {
            Id = id;
            Amount = Math.Max(0, amount);
        }
    }
}