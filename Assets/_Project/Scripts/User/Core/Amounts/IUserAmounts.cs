using LL.Identifiers;
using R3;

namespace LL.User.Core.Amounts
{
    internal interface IUserAmounts<in TId>
        where TId : struct, IIdentifier
    {
        Observable<int> ObserveAmount(TId id);

        bool CanAdd(TId id, int amount);
        bool TryAdd(TId id, int amount);
        bool TrySpend(TId id, int amount);
    }
}