using System;
using LL.Game.Items;
using LL.User.State.Items;
using VContainer;

namespace LL.Game.Cards.Services
{
    internal sealed class CardCollectionService : ICardCollectionService
    {
        private readonly CardCatalog _catalog;
        private readonly IUserItemsCommands _items;

        [Inject]
        internal CardCollectionService(
            CardCatalog catalog,
            IUserItemsCommands items)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _items = items ?? throw new ArgumentNullException(nameof(items));
        }

        public bool TryAdd(ItemId cardId, int amount)
        {
            return _catalog.TryGetCard(cardId, out _) &&
                   _items.TryAdd(cardId, amount);
        }
    }
}