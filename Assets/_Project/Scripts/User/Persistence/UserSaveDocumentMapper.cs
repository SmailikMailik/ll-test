using System;
using System.Linq;
using LL.Game.Items;
using LL.Game.Quests;
using LL.Game.Ranks;
using LL.User.Persistence.Documents;
using LL.User.Snapshots;

namespace LL.User.Persistence
{
    internal static class UserSaveDocumentMapper
    {
        internal static UserSnapshot ToSnapshot(UserSaveDocument saveData)
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
                new UserRankUpQuestSnapshot(
                    new QuestId(saveData.RankUpQuest.QuestId),
                    saveData.RankUpQuest.DeadlineUnixMilliseconds,
                    saveData.RankUpQuest.IsCompleted));
        }

        internal static UserSaveDocument ToDocument(UserSnapshot snapshot)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));

            return new UserSaveDocument(
                UserSaveDocument.CurrentVersion,
                new UserIdentityDocumentEntry(snapshot.Identity.UserId, snapshot.Identity.RegionCode),
                new UserProgressDocumentEntry(
                    snapshot.Progress.RankId.Value,
                    snapshot.Progress.Experience),
                new UserRankUpQuestDocumentEntry(
                    snapshot.RankUpQuest.QuestId.Value,
                    snapshot.RankUpQuest.DeadlineUnixMilliseconds,
                    snapshot.RankUpQuest.IsCompleted),
                snapshot.Items.Amounts
                    .Select(item => new ItemAmountDocumentEntry(item.Id.Value, item.Amount))
                    .ToArray());
        }
    }
}