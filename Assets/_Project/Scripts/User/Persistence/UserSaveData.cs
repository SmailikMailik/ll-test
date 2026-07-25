using Newtonsoft.Json;

namespace LL.User.Persistence
{
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class UserSaveData
    {
        private const int CurrentVersion = 1;

        [JsonProperty] private int _version;
        [JsonProperty] private string _regionCode;
        [JsonProperty] private string _userId;
        [JsonProperty] private int _softAmount;
        [JsonProperty] private int _hardAmount;
        [JsonProperty] private int _masterPointAmount;
        [JsonProperty] private int _totalExperience;

        internal bool IsSupported => _version == CurrentVersion;
        internal string RegionCode => _regionCode;
        internal string UserId => _userId;
        internal int SoftAmount => _softAmount;
        internal int HardAmount => _hardAmount;
        internal int MasterPointAmount => _masterPointAmount;
        internal int TotalExperience => _totalExperience;

        [JsonConstructor]
        private UserSaveData() { }

        internal UserSaveData(
            string regionCode,
            string userId,
            int softAmount,
            int hardAmount,
            int masterPointAmount,
            int totalExperience)
        {
            _version = CurrentVersion;
            _regionCode = regionCode;
            _userId = userId;
            _softAmount = softAmount;
            _hardAmount = hardAmount;
            _masterPointAmount = masterPointAmount;
            _totalExperience = totalExperience;
        }
    }
}