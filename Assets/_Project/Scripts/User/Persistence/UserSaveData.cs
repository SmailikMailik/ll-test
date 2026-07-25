using System;
using System.Collections.Generic;
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
        [JsonProperty] private CurrencySaveData[] _currencies;
        [JsonProperty] private int _totalExperience;
        [JsonProperty] private ExperienceCardSaveData[] _experienceCards;

        internal bool IsSupported => _version == CurrentVersion;
        internal string RegionCode => _regionCode;
        internal string UserId => _userId;
        internal IReadOnlyList<CurrencySaveData> Currencies =>
            _currencies ?? Array.Empty<CurrencySaveData>();
        internal int TotalExperience => _totalExperience;
        internal IReadOnlyList<ExperienceCardSaveData> ExperienceCards =>
            _experienceCards ?? Array.Empty<ExperienceCardSaveData>();

        [JsonConstructor]
        private UserSaveData() { }

        internal UserSaveData(
            string regionCode,
            string userId,
            CurrencySaveData[] currencies,
            int totalExperience,
            ExperienceCardSaveData[] experienceCards)
        {
            _version = CurrentVersion;
            _regionCode = regionCode;
            _userId = userId;
            _currencies = currencies ?? Array.Empty<CurrencySaveData>();
            _totalExperience = totalExperience;
            _experienceCards = experienceCards ?? Array.Empty<ExperienceCardSaveData>();
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class CurrencySaveData
    {
        [JsonProperty] private string _id;
        [JsonProperty] private int _amount;

        internal string Id => _id;
        internal int Amount => _amount;

        [JsonConstructor]
        private CurrencySaveData() { }

        internal CurrencySaveData(string id, int amount)
        {
            _id = id;
            _amount = amount;
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class ExperienceCardSaveData
    {
        [JsonProperty] private string _id;
        [JsonProperty] private int _amount;

        internal string Id => _id;
        internal int Amount => _amount;

        [JsonConstructor]
        private ExperienceCardSaveData() { }

        internal ExperienceCardSaveData(string id, int amount)
        {
            _id = id;
            _amount = amount;
        }
    }
}