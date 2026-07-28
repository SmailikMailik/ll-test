using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Cards;

namespace LL.User.Core.Cards
{
    internal sealed class CardsInitialData
    {
        internal IReadOnlyList<CardAmount> Amounts { get; }

        internal CardsInitialData(IEnumerable<CardAmount> cards)
        {
            var amounts = new Dictionary<CardId, long>();

            if (cards != null)
            {
                foreach (var card in cards)
                {
                    if (card == null || card.Id.IsEmpty)
                        continue;

                    amounts.TryGetValue(card.Id, out var current);
                    amounts[card.Id] = Math.Min(current + card.Amount, int.MaxValue);
                }
            }

            var copy = amounts.Select(pair => new CardAmount(pair.Key, (int)pair.Value)).ToArray();
            Amounts = Array.AsReadOnly(copy);
        }
    }
}