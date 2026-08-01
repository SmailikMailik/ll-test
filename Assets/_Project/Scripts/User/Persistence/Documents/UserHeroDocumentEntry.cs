using System;

namespace LL.User.Persistence.Documents
{
    internal sealed class UserHeroDocumentEntry
    {
        public string HeroId { get; }
        public UserProgressDocumentEntry Progress { get; }
        public UserRankUpAttemptDocumentEntry[] RankUpAttempts { get; }

        public UserHeroDocumentEntry(
            string heroId,
            UserProgressDocumentEntry progress,
            UserRankUpAttemptDocumentEntry[] rankUpAttempts)
        {
            HeroId = heroId;
            Progress = progress ?? throw new ArgumentNullException(nameof(progress));
            RankUpAttempts = rankUpAttempts ?? Array.Empty<UserRankUpAttemptDocumentEntry>();
        }
    }
}