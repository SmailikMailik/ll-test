using System;
using System.Collections.Generic;
using System.Linq;

namespace LL.Game.ExperienceCards
{
    internal sealed class ExperienceCardCatalog
    {
        internal IReadOnlyList<IExperienceCard> Cards { get; }

        internal ExperienceCardCatalog(
            IEnumerable<IExperienceCard> cards)
        {
            var uniqueIds = new HashSet<ExperienceCardId>();
            var copy = cards?
                .Where(card =>
                    card != null &&
                    card.Id.IsEmpty is false &&
                    uniqueIds.Add(card.Id))
                .ToArray()
                ?? Array.Empty<IExperienceCard>();

            Cards = Array.AsReadOnly(copy);
        }
    }
}