using LL.Game.Items;

namespace LL.User.State.Items
{
    internal interface IUserItemsCommands
    {
        bool CanAdd(ItemId id, int amount);
        bool CanSpend(ItemId id, int amount);
        bool TryAdd(ItemId id, int amount);
        bool TrySpend(ItemId id, int amount);
    }
}