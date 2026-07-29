using Newtonsoft.Json;

namespace LL.User.Persistence.SaveData
{
    [JsonObject(MemberSerialization.OptIn, ItemRequired = Required.AllowNull)]
    internal sealed class UserIdentitySaveData
    {
        [JsonProperty] private string _userId;
        [JsonProperty] private string _regionCode;

        internal string UserId => _userId;
        internal string RegionCode => _regionCode;

        [JsonConstructor]
        private UserIdentitySaveData() { }

        internal UserIdentitySaveData(string userId, string regionCode)
        {
            _userId = userId;
            _regionCode = regionCode;
        }
    }
}