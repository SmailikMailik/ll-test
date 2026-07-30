namespace LL.User.Persistence.Documents
{
    internal sealed class UserPromotionQuestDocumentEntry
    {
        public string QuestId { get; }
        public long DeadlineUnixMilliseconds { get; }
        public bool IsCompleted { get; }

        public UserPromotionQuestDocumentEntry(
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