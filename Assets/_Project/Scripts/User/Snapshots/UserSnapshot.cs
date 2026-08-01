using System;

namespace LL.User.Snapshots
{
    internal sealed class UserSnapshot
    {
        internal UserIdentitySnapshot Identity { get; }
        internal UserHeroSelectionSnapshot HeroSelection { get; }
        internal UserProgressSnapshot Progress { get; }
        internal UserRankUpQuestSnapshot RankUpQuest { get; }
        internal UserItemsSnapshot Items { get; }

        internal UserSnapshot(
            UserIdentitySnapshot identity,
            UserHeroSelectionSnapshot heroSelection,
            UserProgressSnapshot progress,
            UserRankUpQuestSnapshot rankUpQuest,
            UserItemsSnapshot items)
        {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            HeroSelection = heroSelection ?? throw new ArgumentNullException(nameof(heroSelection));
            Progress = progress ?? throw new ArgumentNullException(nameof(progress));
            RankUpQuest = rankUpQuest ?? throw new ArgumentNullException(nameof(rankUpQuest));
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }
    }
}