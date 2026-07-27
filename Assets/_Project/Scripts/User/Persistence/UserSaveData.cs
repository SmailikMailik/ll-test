using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LL.User.Persistence
{
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class UserSaveData
    {
        private const int CurrentVersion = 2;

        [JsonProperty] private int _version;
        [JsonProperty] private string _regionCode;
        [JsonProperty] private string _userId;
        [JsonProperty] private CurrencySaveData[] _currencies;
        [JsonProperty] private CardSaveData[] _cards;
        [JsonProperty] private int _rank;
        [JsonProperty] private int _totalExperience;

        internal bool IsSupported => _version == CurrentVersion;
        internal string RegionCode => _regionCode;
        internal string UserId => _userId;
        internal IReadOnlyList<CurrencySaveData> Currencies => _currencies ?? Array.Empty<CurrencySaveData>();
        internal IReadOnlyList<CardSaveData> Cards => _cards ?? Array.Empty<CardSaveData>();
        internal int Rank => _rank;
        internal int TotalExperience => _totalExperience;

        [JsonConstructor]
        private UserSaveData() { }

        internal UserSaveData(
            string regionCode,
            string userId,
            CurrencySaveData[] currencies,
            CardSaveData[] cards,
            int rank,
            int totalExperience)
        {
            _version = CurrentVersion;
            _regionCode = regionCode;
            _userId = userId;
            _currencies = currencies ?? Array.Empty<CurrencySaveData>();
            _cards = cards ?? Array.Empty<CardSaveData>();
            _rank = rank;
            _totalExperience = totalExperience;
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
    internal sealed class CardSaveData
    {
        [JsonProperty] private string _id;
        [JsonProperty] private int _amount;

        internal string Id => _id;
        internal int Amount => _amount;

        [JsonConstructor]
        private CardSaveData() { }

        internal CardSaveData(string id, int amount)
        {
            _id = id;
            _amount = amount;
        }
    }
}