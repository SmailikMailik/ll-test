using System;
using System.Collections.Generic;
using System.Linq;
using LL.Identifiers;

namespace LL.Game.Cards
{
    internal sealed class CardCatalog
    {
        internal IReadOnlyList<ICard> Cards { get; }

        internal CardCatalog(IEnumerable<ICard> cards)
        {
            var copy = cards?.ToArray() ?? Array.Empty<ICard>();
            IdentifierCatalogValidator.EnsureValidIds(
                copy,
                card => card.Id,
                nameof(CardCatalog),
                nameof(cards));

            Cards = Array.AsReadOnly(copy);
        }
    }
}