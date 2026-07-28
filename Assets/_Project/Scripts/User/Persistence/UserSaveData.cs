using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LL.User.Persistence
{
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class UserSaveData
    {
        [JsonProperty] private int _version;
        [JsonProperty] private IdentitySaveData _identity;
        [JsonProperty] private ProgressSaveData _progress;
        [JsonProperty] private CardSaveData[] _cards;
        [JsonProperty] private ItemSaveData[] _items;
        [JsonProperty] private string[] _claimedRewardIds;

        private const int CurrentVersion = 2;

        internal bool IsSupported => _version == CurrentVersion;

        internal IdentitySaveData Identity => _identity;
        internal ProgressSaveData Progress => _progress;
        internal IReadOnlyList<CardSaveData> Cards => _cards ?? Array.Empty<CardSaveData>();
        internal IReadOnlyList<ItemSaveData> Items => _items ?? Array.Empty<ItemSaveData>();
        internal IReadOnlyList<string> ClaimedRewardIds => _claimedRewardIds ?? Array.Empty<string>();

        [JsonConstructor]
        private UserSaveData() { }

        internal UserSaveData(
            IdentitySaveData identity,
            ProgressSaveData progress,
            CardSaveData[] cards,
            ItemSaveData[] items,
            string[] claimedRewardIds)
        {
            _version = CurrentVersion;
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));
            _progress = progress ?? throw new ArgumentNullException(nameof(progress));
            _cards = cards ?? Array.Empty<CardSaveData>();
            _items = items ?? Array.Empty<ItemSaveData>();
            _claimedRewardIds = claimedRewardIds ?? Array.Empty<string>();
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
        [JsonProperty] private int _experience;

        internal int Rank => _rank;
        internal int Experience => _experience;

        [JsonConstructor]
        private ProgressSaveData() { }

        internal ProgressSaveData(int rank, int experience)
        {
            _rank = rank;
            _experience = experience;
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
    internal sealed class ItemSaveData
    {
        [JsonProperty] private string _id;
        [JsonProperty] private int _amount;

        internal string Id => _id;
        internal int Amount => _amount;

        [JsonConstructor]
        private ItemSaveData() { }

        internal ItemSaveData(string id, int amount)
        {
            _id = id;
            _amount = amount;
        }
    }
}