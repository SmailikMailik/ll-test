using System;
using LL.User.Snapshots;

namespace LL.User.Defaults
{
    internal sealed class UserDefaultsSnapshot
    {
        private readonly UserIdentitySnapshot _identity;
        private readonly UserHeroSelectionSnapshot _heroSelection;
        private readonly UserHeroesSnapshot _heroes;
        private readonly UserItemsSnapshot _items;

        internal UserDefaultsSnapshot(
            UserIdentitySnapshot identity,
            UserHeroSelectionSnapshot heroSelection,
            UserHeroesSnapshot heroes,
            UserItemsSnapshot items)
        {
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));
            _heroSelection = heroSelection ?? throw new ArgumentNullException(nameof(heroSelection));
            _heroes = heroes ?? throw new ArgumentNullException(nameof(heroes));
            _items = items ?? throw new ArgumentNullException(nameof(items));
        }

        internal UserSnapshot CreateUserSnapshot()
        {
            return new UserSnapshot(
                _identity,
                _heroSelection,
                _heroes,
                _items);
        }
    }
}