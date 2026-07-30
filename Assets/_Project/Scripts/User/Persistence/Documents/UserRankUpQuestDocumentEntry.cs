namespace LL.User.Persistence.Documents
{
    internal sealed class UserRankUpQuestDocumentEntry
    {
        public string QuestId { get; }
        public long DeadlineUnixMilliseconds { get; }
        public bool IsCompleted { get; }

        public UserRankUpQuestDocumentEntry(
            string questId,
            long deadlineUnixMilliseconds,
            bool isCompleted)
        {
            QuestId = questId;
            DeadlineUnixMilliseconds = deadlineUnixMilliseconds;
            IsCompleted = isCompleted;
        }
    }
}