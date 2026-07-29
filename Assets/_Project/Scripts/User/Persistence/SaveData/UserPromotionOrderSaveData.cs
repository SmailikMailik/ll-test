namespace LL.User.Persistence.SaveData
{
    internal sealed class UserPromotionOrderSaveData
    {
        public string RequirementId { get; }
        public long DeadlineUnixMilliseconds { get; }
        public bool IsCompleted { get; }

        public UserPromotionOrderSaveData(
            string requirementId,
            long deadlineUnixMilliseconds,
            bool isCompleted)
        {
            RequirementId = requirementId;
            DeadlineUnixMilliseconds = deadlineUnixMilliseconds;
            IsCompleted = isCompleted;
        }
    }
}