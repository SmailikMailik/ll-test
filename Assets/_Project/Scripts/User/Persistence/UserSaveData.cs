using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LL.User.Persistence
{
    [JsonObject(MemberSerialization.OptIn, ItemRequired = Required.AllowNull)]
    internal sealed class UserSaveData
    {
        [JsonProperty] private IdentitySaveData _identity;
        [JsonProperty] private ProgressSaveData _progress;
        [JsonProperty] private PromotionOrderSaveData _promotionOrder;
        [JsonProperty] private CardSaveData[] _cards;
        [JsonProperty] private ItemSaveData[] _items;
        [JsonProperty] private string[] _claimedRewardIds;

        internal IdentitySaveData Identity => _identity;
        internal ProgressSaveData Progress => _progress;
        internal PromotionOrderSaveData PromotionOrder => _promotionOrder;
        internal IReadOnlyList<CardSaveData> Cards => _cards;
        internal IReadOnlyList<ItemSaveData> Items => _items;
        internal IReadOnlyList<string> ClaimedRewardIds => _claimedRewardIds;

        [JsonConstructor]
        private UserSaveData() { }

        internal UserSaveData(
            IdentitySaveData identity,
            ProgressSaveData progress,
            PromotionOrderSaveData promotionOrder,
            CardSaveData[] cards,
            ItemSaveData[] items,
            string[] claimedRewardIds)
        {
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));
            _progress = progress ?? throw new ArgumentNullException(nameof(progress));
            _promotionOrder = promotionOrder ?? throw new ArgumentNullException(nameof(promotionOrder));
            _cards = cards ?? Array.Empty<CardSaveData>();
            _items = items ?? Array.Empty<ItemSaveData>();
            _claimedRewardIds = claimedRewardIds ?? Array.Empty<string>();
        }
    }

    [JsonObject(MemberSerialization.OptIn, ItemRequired = Required.AllowNull)]
    internal sealed class IdentitySaveData
    {
        [JsonProperty] private string _userId;
        [JsonProperty] private string _regionCode;

        internal string UserId => _userId;
        internal string RegionCode => _regionCode;

        [JsonConstructor]
        private IdentitySaveData() { }

        internal IdentitySaveData(string userId, string regionCode)
        {
            _userId = userId;
            _regionCode = regionCode;
        }
    }

    [JsonObject(MemberSerialization.OptIn, ItemRequired = Required.AllowNull)]
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

    [JsonObject(MemberSerialization.OptIn, ItemRequired = Required.AllowNull)]
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

    [JsonObject(MemberSerialization.OptIn, ItemRequired = Required.AllowNull)]
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

    [JsonObject(MemberSerialization.OptIn, ItemRequired = Required.AllowNull)]
    internal sealed class PromotionOrderSaveData
    {
        [JsonProperty] private string _requirementId;
        [JsonProperty] private long _deadlineUnixMilliseconds;
        [JsonProperty] private bool _isCompleted;

        internal string RequirementId => _requirementId;
        internal long DeadlineUnixMilliseconds => _deadlineUnixMilliseconds;
        internal bool IsCompleted => _isCompleted;

        [JsonConstructor]
        private PromotionOrderSaveData() { }

        internal PromotionOrderSaveData(
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