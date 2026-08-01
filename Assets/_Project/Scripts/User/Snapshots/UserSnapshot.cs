using System;

namespace LL.User.Snapshots
{
    internal sealed class UserSnapshot
    {
        internal UserIdentitySnapshot Identity { get; }
        internal UserHeroSelectionSnapshot HeroSelection { get; }
        internal UserHeroesSnapshot Heroes { get; }
        internal UserItemsSnapshot Items { get; }

        internal UserSnapshot(
            UserIdentitySnapshot identity,
            UserHeroSelectionSnapshot heroSelection,
            UserHeroesSnapshot heroes,
            UserItemsSnapshot items)
        {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            HeroSelection = heroSelection ?? throw new ArgumentNullException(nameof(heroSelection));
            Heroes = heroes ?? throw new ArgumentNullException(nameof(heroes));
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }
    }
}