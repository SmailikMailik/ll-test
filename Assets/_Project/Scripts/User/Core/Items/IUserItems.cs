using LL.Game.Items;
using R3;

namespace LL.User.Core.Items
{
    internal interface IUserItems
    {
        Observable<int> ObserveAmount(ItemId id);

        bool CanAdd(ItemId id, int amount);
        bool TryAdd(ItemId id, int amount);
        bool TrySpend(ItemId id, int amount);
    }
}