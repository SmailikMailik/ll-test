using System;
using System.Linq;
using LL.Game.Heroes;
using LL.Game.Items;
using LL.Game.Ranks;
using LL.Game.RankUp;
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
                ToUserHeroSelectionSnapshot(document.HeroSelection),
                new UserHeroesSnapshot(document.Heroes.Select(ToUserHeroSnapshot)),
                new UserItemsSnapshot(document.Items.Select(ToItemAmount)));
        }

        internal static UserSaveDocument ToDocument(UserSnapshot snapshot)
        {
            if (snapshot is null)
                throw new ArgumentNullException(nameof(snapshot));

            return new UserSaveDocument(
                UserSaveDocument.CurrentVersion,
                ToUserIdentityDocumentEntry(snapshot.Identity),
                ToUserHeroSelectionDocumentEntry(snapshot.HeroSelection),
                snapshot.Heroes.Heroes.Select(ToUserHeroDocumentEntry).ToArray(),
                snapshot.Items.Amounts.Select(ToUserItemDocumentEntry).ToArray());
        }

        private static UserIdentitySnapshot ToUserIdentitySnapshot(UserIdentityDocumentEntry identity)
        {
            return new UserIdentitySnapshot(identity.UserId, identity.RegionCode);
        }

        private static UserHeroSelectionSnapshot ToUserHeroSelectionSnapshot(
            UserHeroSelectionDocumentEntry heroSelection)
        {
            return new UserHeroSelectionSnapshot(new HeroId(heroSelection.HeroId));
        }

        private static UserHeroSnapshot ToUserHeroSnapshot(UserHeroDocumentEntry hero)
        {
            return new UserHeroSnapshot(
                new HeroId(hero.HeroId),
                ToUserProgressSnapshot(hero.Progress),
                hero.RankUpAttempts.Select(ToUserRankUpAttemptSnapshot));
        }

        private static UserProgressSnapshot ToUserProgressSnapshot(UserProgressDocumentEntry progress)
        {
            return new UserProgressSnapshot(new RankId(progress.RankId), progress.Experience);
        }

        private static UserRankUpAttemptSnapshot ToUserRankUpAttemptSnapshot(UserRankUpAttemptDocumentEntry attempt)
        {
            return new UserRankUpAttemptSnapshot(
                new RankId(attempt.RankId),
                new RankUpOptionId(attempt.OptionId),
                attempt.Quests.Select(ToUserRankUpQuestRequirementSnapshot));
        }

        private static UserRankUpQuestRequirementSnapshot ToUserRankUpQuestRequirementSnapshot(
            UserRankUpQuestRequirementDocumentEntry quest)
        {
            return new UserRankUpQuestRequirementSnapshot(
                new RankUpRequirementId(quest.RequirementId),
                quest.CurrentCount,
                quest.DeadlineUnixMilliseconds);
        }

        private static ItemAmount ToItemAmount(UserItemDocumentEntry item)
        {
            return new ItemAmount(new ItemId(item.Id), item.Amount);
        }

        private static UserIdentityDocumentEntry ToUserIdentityDocumentEntry(UserIdentitySnapshot identity)
        {
            return new UserIdentityDocumentEntry(identity.UserId, identity.RegionCode);
        }

        private static UserHeroSelectionDocumentEntry ToUserHeroSelectionDocumentEntry(
            UserHeroSelectionSnapshot heroSelection)
        {
            return new UserHeroSelectionDocumentEntry(heroSelection.HeroId.Value);
        }

        private static UserHeroDocumentEntry ToUserHeroDocumentEntry(UserHeroSnapshot hero)
        {
            return new UserHeroDocumentEntry(
                hero.HeroId.Value,
                ToUserProgressDocumentEntry(hero.Progress),
                hero.RankUpAttempts.Select(ToUserRankUpAttemptDocumentEntry).ToArray());
        }

        private static UserProgressDocumentEntry ToUserProgressDocumentEntry(UserProgressSnapshot progress)
        {
            return new UserProgressDocumentEntry(progress.RankId.Value, progress.Experience);
        }

        private static UserRankUpAttemptDocumentEntry ToUserRankUpAttemptDocumentEntry(UserRankUpAttemptSnapshot attempt)
        {
            return new UserRankUpAttemptDocumentEntry(
                attempt.RankId.Value,
                attempt.OptionId.Value,
                attempt.Quests.Select(ToUserRankUpQuestRequirementDocumentEntry).ToArray());
        }

        private static UserRankUpQuestRequirementDocumentEntry ToUserRankUpQuestRequirementDocumentEntry(
            UserRankUpQuestRequirementSnapshot quest)
        {
            return new UserRankUpQuestRequirementDocumentEntry(
                quest.RequirementId.Value,
                quest.CurrentCount,
                quest.DeadlineUnixMilliseconds);
        }

        private static UserItemDocumentEntry ToUserItemDocumentEntry(ItemAmount item)
        {
            return new UserItemDocumentEntry(item.Id.Value, item.Amount);
        }
    }
}