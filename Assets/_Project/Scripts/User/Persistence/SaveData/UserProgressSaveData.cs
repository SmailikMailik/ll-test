using Newtonsoft.Json;

namespace LL.User.Persistence.SaveData
{
    [JsonObject(MemberSerialization.OptIn, ItemRequired = Required.AllowNull)]
    internal sealed class UserProgressSaveData
    {
        [JsonProperty] private int _rank;
        [JsonProperty] private int _experience;

        internal int Rank => _rank;
        internal int Experience => _experience;

        [JsonConstructor]
        private UserProgressSaveData() { }

        internal UserProgressSaveData(int rank, int experience)
        {
            _rank = rank;
            _experience = experience;
        }
    }
}