using System;
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
        internal static UserData ToUserData(UserSaveData saveData)
        {
            if (saveData == null)
                throw new ArgumentNullException(nameof(saveData));

            return new UserData(
                new UserIdentity(saveData.Identity.UserId, saveData.Identity.RegionCode),
                new UserItemsData(
                    saveData.Items.Select(item =>
                        new ItemAmount(new ItemId(item.Id), item.Amount))),
                new UserProgressData(saveData.Progress.Rank, saveData.Progress.Experience),
                new UserPromotionOrderData(
                    new PromotionRequirementId(saveData.PromotionOrder.RequirementId),
                    saveData.PromotionOrder.DeadlineUnixMilliseconds,
                    saveData.PromotionOrder.IsCompleted),
                new UserRewardClaimsData(
                    saveData.ClaimedRewardIds.Select(id => new RewardBundleId(id))));
        }

        internal static UserSaveData ToSaveData(UserData userData)
        {
            if (userData == null)
                throw new ArgumentNullException(nameof(userData));

            return new UserSaveData(
                new UserIdentitySaveData(userData.Identity.UserId, userData.Identity.RegionCode),
                new UserProgressSaveData(userData.Progress.Rank, userData.Progress.Experience),
                new UserPromotionOrderSaveData(
                    userData.PromotionOrder.RequirementId.Value,
                    userData.PromotionOrder.DeadlineUnixMilliseconds,
                    userData.PromotionOrder.IsCompleted),
                userData.Items.Amounts
                    .Select(item => new ItemAmountSaveData(item.Id.Value, item.Amount))
                    .ToArray(),
                userData.RewardClaims.ClaimedIds
                    .Where(id => id.IsEmpty is false)
                    .Select(id => id.Value)
                    .ToArray());
        }
    }
}