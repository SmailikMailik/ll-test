using R3;

namespace LL.User.Core.Wallet
{
    internal interface IUserWallet
    {
        Observable<int> ObserveAmount(CurrencyType type);

        bool TryAdd(CurrencyType type, int amount);
        bool TrySpend(CurrencyType type, int amount);
    }
}