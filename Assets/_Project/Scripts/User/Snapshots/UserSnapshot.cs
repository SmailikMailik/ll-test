using System;

namespace LL.User.Snapshots
{
    internal sealed class UserSnapshot
    {
        internal UserIdentitySnapshot Identity { get; }
        internal UserItemsSnapshot Items { get; }
        internal UserProgressSnapshot Progress { get; }
        internal UserPromotionOrderSnapshot PromotionOrder { get; }
        internal UserRewardClaimsSnapshot RewardClaims { get; }

        internal UserSnapshot(
            UserIdentitySnapshot identity,
            UserItemsSnapshot items,
            UserProgressSnapshot progress,
            UserPromotionOrderSnapshot promotionOrder,
            UserRewardClaimsSnapshot rewardClaims)
        {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            Items = items ?? throw new ArgumentNullException(nameof(items));
            Progress = progress ?? throw new ArgumentNullException(nameof(progress));
            PromotionOrder = promotionOrder ?? throw new ArgumentNullException(nameof(promotionOrder));
            RewardClaims = rewardClaims ?? throw new ArgumentNullException(nameof(rewardClaims));
        }
    }
}