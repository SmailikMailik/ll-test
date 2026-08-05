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
    }
}