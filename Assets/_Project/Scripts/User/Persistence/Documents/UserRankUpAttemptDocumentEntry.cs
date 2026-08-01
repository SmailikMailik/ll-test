using System;

namespace LL.User.Persistence.Documents
{
    internal sealed class UserRankUpAttemptDocumentEntry
    {
        public string RankId { get; }
        public string OptionId { get; }
        public UserRankUpQuestRequirementDocumentEntry[] Quests { get; }

        public UserRankUpAttemptDocumentEntry(
            string rankId,
            string optionId,
            UserRankUpQuestRequirementDocumentEntry[] quests)
        {
            RankId = rankId;
            OptionId = optionId;
            Quests = quests ?? Array.Empty<UserRankUpQuestRequirementDocumentEntry>();
        }
    }

    internal sealed class UserRankUpQuestRequirementDocumentEntry
    {
        public string RequirementId { get; }
        public int CurrentCount { get; }
        public long DeadlineUnixMilliseconds { get; }

        public UserRankUpQuestRequirementDocumentEntry(
            string requirementId,
            int currentCount,
            long deadlineUnixMilliseconds)
        {
            RequirementId = requirementId;
            CurrentCount = currentCount;
            DeadlineUnixMilliseconds = deadlineUnixMilliseconds;
        }
    }
}