using System;
using System.Linq;
using LL.Game.Items;
using LL.Game.Promotions;
using LL.Game.Rewards;
using LL.User.Persistence.SaveData;
using LL.User.Snapshots;

namespace LL.User.Persistence
{
    internal static class UserSaveDataMapper
    {
        internal static UserSnapshot ToSnapshot(UserSaveData saveData)
        {
            if (saveData == null)
                throw new ArgumentNullException(nameof(saveData));

            return new UserSnapshot(
                new UserIdentitySnapshot(saveData.Identity.UserId, saveData.Identity.RegionCode),
                new UserItemsSnapshot(
                    saveData.Items.Select(item =>
                        new ItemAmount(new ItemId(item.Id), item.Amount))),
                new UserProgressSnapshot(saveData.Progress.Rank, saveData.Progress.Experience),
                new UserPromotionOrderSnapshot(
                    new PromotionRequirementId(saveData.PromotionOrder.RequirementId),
                    saveData.PromotionOrder.DeadlineUnixMilliseconds,
                    saveData.PromotionOrder.IsCompleted),
                new UserRewardClaimsSnapshot(
                    saveData.ClaimedRewardIds.Select(id => new RewardBundleId(id))));
        }

        internal static UserSaveData ToSaveData(UserSnapshot snapshot)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));

            return new UserSaveData(
                UserSaveData.CurrentVersion,
                new UserIdentitySaveData(snapshot.Identity.UserId, snapshot.Identity.RegionCode),
                new UserProgressSaveData(snapshot.Progress.Rank, snapshot.Progress.Experience),
                new UserPromotionOrderSaveData(
                    snapshot.PromotionOrder.RequirementId.Value,
                    snapshot.PromotionOrder.DeadlineUnixMilliseconds,
                    snapshot.PromotionOrder.IsCompleted),
                snapshot.Items.Amounts
                    .Select(item => new ItemAmountSaveData(item.Id.Value, item.Amount))
                    .ToArray(),
                snapshot.RewardClaims.ClaimedIds
                    .Select(id => id.Value)
                    .ToArray());
        }
    }
}