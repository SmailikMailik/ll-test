using System;

namespace LL.User.Persistence.Documents
{
    internal sealed class UserSaveDocument
    {
        internal const int CurrentVersion = 1;

        public int Version { get; }
        public UserIdentityDocumentEntry Identity { get; }
        public UserHeroSelectionDocumentEntry HeroSelection { get; }
        public UserProgressDocumentEntry Progress { get; }
        public UserRankUpQuestDocumentEntry RankUpQuest { get; }
        public UserItemDocumentEntry[] Items { get; }

        public UserSaveDocument(
            int version,
            UserIdentityDocumentEntry identity,
            UserHeroSelectionDocumentEntry heroSelection,
            UserProgressDocumentEntry progress,
            UserRankUpQuestDocumentEntry rankUpQuest,
            UserItemDocumentEntry[] items)
        {
            Version = version;
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            HeroSelection = heroSelection ?? throw new ArgumentNullException(nameof(heroSelection));
            Progress = progress ?? throw new ArgumentNullException(nameof(progress));
            RankUpQuest = rankUpQuest ?? throw new ArgumentNullException(nameof(rankUpQuest));
            Items = items ?? Array.Empty<UserItemDocumentEntry>();
        }
    }
}