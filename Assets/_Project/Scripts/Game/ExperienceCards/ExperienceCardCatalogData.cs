using System;
using System.Collections.Generic;
using System.Linq;

namespace LL.Game.ExperienceCards
{
    internal sealed class ExperienceCardCatalogData
    {
        internal IReadOnlyList<ExperienceCardDefinitionData> Cards { get; }

        internal ExperienceCardCatalogData(
            IEnumerable<ExperienceCardDefinitionData> cards)
        {
            var uniqueIds = new HashSet<ExperienceCardId>();
            var copy = cards?
                .Where(card =>
                    card != null &&
                    card.Id.IsEmpty is false &&
                    uniqueIds.Add(card.Id))
                .ToArray()
                ?? Array.Empty<ExperienceCardDefinitionData>();

            Cards = Array.AsReadOnly(copy);
        }
    }
}