using System;
using LL.User.Snapshots;

namespace LL.User.Defaults
{
    internal sealed class UserDefaultsSnapshot
    {
        private readonly UserIdentitySnapshot _identity;
        private readonly UserHeroSelectionSnapshot _heroSelection;
        private readonly UserProgressSnapshot _progress;
        private readonly UserItemsSnapshot _items;

        internal UserDefaultsSnapshot(
            UserIdentitySnapshot identity,
            UserHeroSelectionSnapshot heroSelection,
            UserProgressSnapshot progress,
            UserItemsSnapshot items)
        {
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));
            _heroSelection = heroSelection ?? throw new ArgumentNullException(nameof(heroSelection));
            _progress = progress ?? throw new ArgumentNullException(nameof(progress));
            _items = items ?? throw new ArgumentNullException(nameof(items));
        }

        internal UserSnapshot CreateUserSnapshot()
        {
            return new UserSnapshot(
                _identity,
                _heroSelection,
                _progress,
                UserRankUpQuestSnapshot.Empty,
                _items);
        }
    }
}