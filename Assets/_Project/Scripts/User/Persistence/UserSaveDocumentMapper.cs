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
                ToUserIdentitySnapshot(document.Identity),
                ToUserProgressSnapshot(document.Progress),
                ToUserRankUpQuestSnapshot(document.RankUpQuest),
                new UserItemsSnapshot(document.Items.Select(ToItemAmount)));
        }

        internal static UserSaveDocument ToDocument(UserSnapshot snapshot)
        {
            if (snapshot is null)
                throw new ArgumentNullException(nameof(snapshot));

            return new UserSaveDocument(
                UserSaveDocument.CurrentVersion,
                ToUserIdentityDocumentEntry(snapshot.Identity),
                ToUserProgressDocumentEntry(snapshot.Progress),
                ToUserRankUpQuestDocumentEntry(snapshot.RankUpQuest),
                snapshot.Items.Amounts
                    .Select(ToUserItemDocumentEntry)
                    .ToArray());
        }

        private static UserIdentitySnapshot ToUserIdentitySnapshot(UserIdentityDocumentEntry identity)
        {
            return new UserIdentitySnapshot(identity.UserId, identity.RegionCode);
        }

        private static UserProgressSnapshot ToUserProgressSnapshot(UserProgressDocumentEntry progress)
        {
            return new UserProgressSnapshot(new RankId(progress.RankId), progress.Experience);
        }

        private static UserRankUpQuestSnapshot ToUserRankUpQuestSnapshot(UserRankUpQuestDocumentEntry rankUpQuest)
        {
            return new UserRankUpQuestSnapshot(
                new QuestId(rankUpQuest.QuestId),
                rankUpQuest.DeadlineUnixMilliseconds,
                rankUpQuest.IsCompleted);
        }

        private static ItemAmount ToItemAmount(UserItemDocumentEntry item)
        {
            return new ItemAmount(new ItemId(item.Id), item.Amount);
        }

        private static UserIdentityDocumentEntry ToUserIdentityDocumentEntry(UserIdentitySnapshot identity)
        {
            return new UserIdentityDocumentEntry(identity.UserId, identity.RegionCode);
        }

        private static UserProgressDocumentEntry ToUserProgressDocumentEntry(UserProgressSnapshot progress)
        {
            return new UserProgressDocumentEntry(progress.RankId.Value, progress.Experience);
        }

        private static UserRankUpQuestDocumentEntry ToUserRankUpQuestDocumentEntry(UserRankUpQuestSnapshot rankUpQuest)
        {
            return new UserRankUpQuestDocumentEntry(
                rankUpQuest.QuestId.Value,
                rankUpQuest.DeadlineUnixMilliseconds,
                rankUpQuest.IsCompleted);
        }

        private static UserItemDocumentEntry ToUserItemDocumentEntry(ItemAmount item)
        {
            return new UserItemDocumentEntry(item.Id.Value, item.Amount);
        }
    }
}