using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Items;
using LL.Game.Promotions;
using LL.Rewards.Models;
using LL.User.Core;
using LL.User.Core.Items;
using LL.User.Core.Identity;
using LL.User.Core.Promotions;
using LL.User.Core.Progress;
using LL.User.Core.Rewards;

namespace LL.User.Persistence
{
    internal static class UserSaveDataMapper
    {
        internal static UserInitialData ToInitialData(UserSaveData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return new UserInitialData(
                new UserIdentity(data.Identity.UserId, data.Identity.RegionCode),
                new UserItemsInitialData(
                    data.Items.Select(item =>
                        new ItemAmount(new ItemId(item.Id), item.Amount))),
                new UserProgressInitialData(data.Progress.Rank, data.Progress.Experience),
                new UserPromotionOrderInitialData(
                    new PromotionRequirementId(data.PromotionOrder.RequirementId),
                    data.PromotionOrder.DeadlineUnixMilliseconds,
                    data.PromotionOrder.IsCompleted),
                new UserRewardClaimsInitialData(
                    data.ClaimedRewardIds.Select(id => new RewardBundleId(id))));
        }

        internal static UserSaveData ToSaveData(UserInitialData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return ToSaveData(
                data.Identity,
                data.Progress,
                data.Items.Amounts,
                data.PromotionOrder,
                data.RewardClaims.ClaimedIds);
        }

        internal static UserSaveData ToSaveData(
            UserIdentity identity,
            UserProgressInitialData progress,
            IEnumerable<ItemAmount> items,
            UserPromotionOrderInitialData promotionOrder,
            IEnumerable<RewardBundleId> claimedRewardIds)
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));

            if (progress == null)
                throw new ArgumentNullException(nameof(progress));

            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (promotionOrder == null)
                throw new ArgumentNullException(nameof(promotionOrder));

            if (claimedRewardIds == null)
                throw new ArgumentNullException(nameof(claimedRewardIds));

            return new UserSaveData(
                new UserIdentitySaveData(identity.UserId, identity.RegionCode),
                new UserProgressSaveData(progress.Rank, progress.Experience),
                new UserPromotionOrderSaveData(
                    promotionOrder.RequirementId.Value,
                    promotionOrder.DeadlineUnixMilliseconds,
                    promotionOrder.IsCompleted),
                items
                    .Select(item => new ItemAmountSaveData(item.Id.Value, item.Amount))
                    .ToArray(),
                claimedRewardIds
                    .Where(id => id.IsEmpty is false)
                    .Select(id => id.Value)
                    .ToArray());
        }
    }
}