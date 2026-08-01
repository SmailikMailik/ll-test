namespace LL.User.Persistence.Documents
{
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