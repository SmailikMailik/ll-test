using Newtonsoft.Json;

namespace LL.User.Persistence.SaveData
{
    [JsonObject(MemberSerialization.OptIn, ItemRequired = Required.AllowNull)]
    internal sealed class UserPromotionOrderSaveData
    {
        [JsonProperty] private string _requirementId;
        [JsonProperty] private long _deadlineUnixMilliseconds;
        [JsonProperty] private bool _isCompleted;

        internal string RequirementId => _requirementId;
        internal long DeadlineUnixMilliseconds => _deadlineUnixMilliseconds;
        internal bool IsCompleted => _isCompleted;

        [JsonConstructor]
        private UserPromotionOrderSaveData() { }

        internal UserPromotionOrderSaveData(
            string requirementId,
            long deadlineUnixMilliseconds,
            bool isCompleted)
        {
            _requirementId = requirementId;
            _deadlineUnixMilliseconds = deadlineUnixMilliseconds;
            _isCompleted = isCompleted;
        }
    }
}