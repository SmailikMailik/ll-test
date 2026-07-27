using LL.Game.Cards;
using R3;

namespace LL.User.Core.Cards
{
    internal interface IUserCards
    {
        Observable<int> ObserveAmount(CardId id);

        bool TryAdd(CardId id, int amount);
    }
}