using System;

namespace LL.User.Snapshots
{
    internal sealed class UserSnapshot
    {
        internal UserIdentitySnapshot Identity { get; }
        internal UserItemsSnapshot Items { get; }
        internal UserProgressSnapshot Progress { get; }
        internal UserPromotionQuestSnapshot PromotionQuest { get; }

        internal UserSnapshot(
            UserIdentitySnapshot identity,
            UserItemsSnapshot items,
            UserProgressSnapshot progress,
            UserPromotionQuestSnapshot promotionQuest)
        {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            Items = items ?? throw new ArgumentNullException(nameof(items));
            Progress = progress ?? throw new ArgumentNullException(nameof(progress));
            PromotionQuest = promotionQuest ?? throw new ArgumentNullException(nameof(promotionQuest));
        }
    }
}