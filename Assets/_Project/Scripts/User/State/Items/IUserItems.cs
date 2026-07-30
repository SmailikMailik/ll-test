using LL.Game.Items;
using LL.User.Snapshots;
using R3;

namespace LL.User.State.Items
{
    internal interface IUserItems
    {
        Observable<Unit> Changed { get; }

        Observable<int> ObserveAmount(ItemId id);
        UserItemsSnapshot CreateSnapshot();

        bool CanAdd(ItemId id, int amount);
        bool CanSpend(ItemId id, int amount);

        bool TryAdd(ItemId id, int amount);
        bool TrySpend(ItemId id, int amount);
    }
}