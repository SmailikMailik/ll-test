using LL.Game.Currencies;
using R3;

namespace LL.User.Core.Wallet
{
    internal interface IUserWallet
    {
        Observable<int> ObserveAmount(CurrencyId id);

        bool CanAdd(CurrencyId id, int amount);
        bool TryAdd(CurrencyId id, int amount);
        bool TrySpend(CurrencyId id, int amount);
    }
}