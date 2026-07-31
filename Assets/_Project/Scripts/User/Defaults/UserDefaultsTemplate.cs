using System;
using LL.User.Snapshots;

namespace LL.User.Defaults
{
    internal sealed class UserDefaultsTemplate
    {
        private readonly UserIdentitySnapshot _identity;
        private readonly UserItemsSnapshot _items;
        private readonly UserProgressSnapshot _progress;

        internal UserDefaultsTemplate(
            UserIdentitySnapshot identity,
            UserItemsSnapshot items,
            UserProgressSnapshot progress)
        {
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));
            _items = items ?? throw new ArgumentNullException(nameof(items));
            _progress = progress ?? throw new ArgumentNullException(nameof(progress));
        }

        internal UserSnapshot CreateSnapshot()
        {
            return new UserSnapshot(
                _identity,
                _items,
                _progress,
                UserRankUpQuestSnapshot.Empty);
        }
    }
}