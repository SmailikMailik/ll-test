using System;
using LL.Game.Cards;

namespace LL.User.Core.Cards
{
    internal sealed class CardStack
    {
        internal CardId Id { get; }
        internal int Amount { get; }

        internal CardStack(CardId id, int amount)
        {
            Id = id;
            Amount = Math.Max(0, amount);
        }
    }
}