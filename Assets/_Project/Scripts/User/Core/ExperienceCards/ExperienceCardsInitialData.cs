using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.ExperienceCards;

namespace LL.User.Core.ExperienceCards
{
    internal sealed class ExperienceCardsInitialData
    {
        internal IReadOnlyList<ExperienceCardAmountData> Cards { get; }

        internal ExperienceCardsInitialData(
            IEnumerable<ExperienceCardAmountData> cards)
        {
            var amounts = new Dictionary<ExperienceCardId, long>();

            if (cards != null)
            {
                foreach (var card in cards)
                {
                    if (card == null || card.Id.IsEmpty)
                        continue;

                    amounts.TryGetValue(card.Id, out var current);
                    amounts[card.Id] = Math.Min(
                        current + card.Amount,
                        int.MaxValue);
                }
            }

            var copy = amounts
                .Select(pair => new ExperienceCardAmountData(
                    pair.Key,
                    (int)pair.Value))
                .ToArray();

            Cards = Array.AsReadOnly(copy);
        }
    }
}