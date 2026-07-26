using System;
using System.Collections.Generic;
using System.Linq;
using LL.Identifiers;

namespace LL.Game.ExperienceCards
{
    internal sealed class ExperienceCardCatalog
    {
        internal IReadOnlyList<IExperienceCard> Cards { get; }

        internal ExperienceCardCatalog(IEnumerable<IExperienceCard> cards)
        {
            var copy = cards?.ToArray() ?? Array.Empty<IExperienceCard>();
            IdentifierCatalogValidator.EnsureValidIds(
                copy,
                card => card.Id,
                nameof(ExperienceCardCatalog),
                nameof(cards));

            Cards = Array.AsReadOnly(copy);
        }
    }
}