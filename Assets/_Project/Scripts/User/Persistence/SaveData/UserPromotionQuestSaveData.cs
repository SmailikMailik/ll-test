namespace LL.User.Persistence.SaveData
{
    internal sealed class UserPromotionQuestSaveData
    {
        public string QuestId { get; }
        public long DeadlineUnixMilliseconds { get; }
        public bool IsCompleted { get; }

        public UserPromotionQuestSaveData(
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