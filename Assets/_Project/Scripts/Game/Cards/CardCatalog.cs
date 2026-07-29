using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Items;
using LL.Game.Identifiers;

namespace LL.Game.Cards
{
    internal sealed class CardCatalog
    {
        private readonly IReadOnlyDictionary<ItemId, ICard> _cardsById;

        internal CardCatalog(IEnumerable<ICard> cards)
        {
            var copy = cards?.ToArray() ?? Array.Empty<ICard>();
            IdentifierCatalogValidator.EnsureValidIds(
                copy,
                card => card.Id,
                nameof(CardCatalog),
                nameof(cards));

            _cardsById = copy.ToDictionary(card => card.Id);
        }

        internal bool TryGetCard(ItemId id, out ICard card) => _cardsById.TryGetValue(id, out card);
    }
}