using System;
using System.Linq;
using LL.Game.Items;
using LL.Game.Quests;
using LL.Game.Ranks;
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
                new UserProgressSnapshot(
                    new RankId(saveData.Progress.RankId),
                    saveData.Progress.Experience),
                new UserPromotionQuestSnapshot(
                    new QuestId(saveData.PromotionQuest.QuestId),
                    saveData.PromotionQuest.DeadlineUnixMilliseconds,
                    saveData.PromotionQuest.IsCompleted));
        }

        internal static UserSaveData ToSaveData(UserSnapshot snapshot)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));

            return new UserSaveData(
                UserSaveData.CurrentVersion,
                new UserIdentitySaveData(snapshot.Identity.UserId, snapshot.Identity.RegionCode),
                new UserProgressSaveData(
                    snapshot.Progress.RankId.Value,
                    snapshot.Progress.Experience),
                new UserPromotionQuestSaveData(
                    snapshot.PromotionQuest.QuestId.Value,
                    snapshot.PromotionQuest.DeadlineUnixMilliseconds,
                    snapshot.PromotionQuest.IsCompleted),
                snapshot.Items.Amounts
                    .Select(item => new ItemAmountSaveData(item.Id.Value, item.Amount))
                    .ToArray());
        }
    }
}