using LL.Game.Items;

namespace LL.Game.Cards.Services
{
    internal interface ICardCollectionService
    {
        bool TryAdd(ItemId cardId, int amount);
    }
}