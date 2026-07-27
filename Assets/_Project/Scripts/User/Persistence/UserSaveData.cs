using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LL.User.Persistence
{
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class UserSaveData
    {
        private const int CurrentVersion = 3;

        [JsonProperty] private int _version;
        [JsonProperty] private IdentitySaveData _identity;
        [JsonProperty] private ProgressSaveData _progress;
        [JsonProperty] private CardSaveData[] _cards;
        [JsonProperty] private CurrencySaveData[] _currencies;

        internal bool IsSupported => _version == CurrentVersion;

        internal IdentitySaveData Identity => _identity;
        internal ProgressSaveData Progress => _progress;
        internal IReadOnlyList<CardSaveData> Cards => _cards ?? Array.Empty<CardSaveData>();
        internal IReadOnlyList<CurrencySaveData> Currencies => _currencies ?? Array.Empty<CurrencySaveData>();

        [JsonConstructor]
        private UserSaveData() { }

        internal UserSaveData(
            IdentitySaveData identity,
            ProgressSaveData progress,
            CardSaveData[] cards,
            CurrencySaveData[] currencies)
        {
            _version = CurrentVersion;
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));
            _progress = progress ?? throw new ArgumentNullException(nameof(progress));
            _cards = cards ?? Array.Empty<CardSaveData>();
            _currencies = currencies ?? Array.Empty<CurrencySaveData>();
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class IdentitySaveData
    {
        [JsonProperty] private string _regionCode;
        [JsonProperty] private string _userId;

        internal string RegionCode => _regionCode;
        internal string UserId => _userId;

        [JsonConstructor]
        private IdentitySaveData() { }

        internal IdentitySaveData(string regionCode, string userId)
        {
            _regionCode = regionCode;
            _userId = userId;
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class ProgressSaveData
    {
        [JsonProperty] private int _rank;
        [JsonProperty] private int _totalExperience;

        internal int Rank => _rank;
        internal int TotalExperience => _totalExperience;

        [JsonConstructor]
        private ProgressSaveData() { }

        internal ProgressSaveData(int rank, int totalExperience)
        {
            _rank = rank;
            _totalExperience = totalExperience;
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
}