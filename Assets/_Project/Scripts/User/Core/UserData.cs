using System;
using LL.User.Core.Items;
using LL.User.Core.Identity;
using LL.User.Core.Promotions;
using LL.User.Core.Progress;
using LL.User.Core.Rewards;

namespace LL.User.Core
{
    internal sealed class UserData
    {
        internal UserIdentity Identity { get; }
        internal UserItemsData Items { get; }
        internal UserProgressData Progress { get; }
        internal UserPromotionOrderData PromotionOrder { get; }
        internal UserRewardClaimsData RewardClaims { get; }

        internal UserData(
            UserIdentity identity,
            UserItemsData items,
            UserProgressData progress,
            UserPromotionOrderData promotionOrder,
            UserRewardClaimsData rewardClaims)
        {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            Items = items ?? throw new ArgumentNullException(nameof(items));
            Progress = progress ?? throw new ArgumentNullException(nameof(progress));
            PromotionOrder = promotionOrder ?? throw new ArgumentNullException(nameof(promotionOrder));
            RewardClaims = rewardClaims ?? throw new ArgumentNullException(nameof(rewardClaims));
        }
    }
}