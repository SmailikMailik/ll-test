using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Identifiers;
using LL.Game.Items;

namespace LL.Game.Cards
{
    internal sealed class CardCatalog
    {
        internal IReadOnlyList<ICard> Cards { get; }

        private readonly IReadOnlyDictionary<ItemId, ICard> _cardsById;

        internal CardCatalog(IEnumerable<ICard> cards)
        {
            var entries = cards?.ToArray() ?? Array.Empty<ICard>();
            IdentifierCollectionValidator.EnsureValid(
                entries,
                card => card.Id,
                nameof(cards));

            Cards = Array.AsReadOnly(entries);
            _cardsById = entries.ToDictionary(card => card.Id);
        }

        internal bool TryGetCard(ItemId id, out ICard card) => _cardsById.TryGetValue(id, out card);
    }
}