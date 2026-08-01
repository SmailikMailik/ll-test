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
        internal static UserSnapshot ToSnapshot(UserSaveDocument document)
        {
            if (document is null)
                throw new ArgumentNullException(nameof(document));

            return new UserSnapshot(
                new UserIdentitySnapshot(document.Identity.UserId, document.Identity.RegionCode),
                new UserItemsSnapshot(
                    document.Items.Select(item =>
                        new ItemAmount(new ItemId(item.Id), item.Amount))),
                new UserProgressSnapshot(
                    new RankId(document.Progress.RankId),
                    document.Progress.Experience),
                new UserRankUpQuestSnapshot(
                    new QuestId(document.RankUpQuest.QuestId),
                    document.RankUpQuest.DeadlineUnixMilliseconds,
                    document.RankUpQuest.IsCompleted));
        }

        internal static UserSaveDocument ToDocument(UserSnapshot snapshot)
        {
            if (snapshot is null)
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
                    .Select(item => new UserItemDocumentEntry(item.Id.Value, item.Amount))
                    .ToArray());
        }
    }
}