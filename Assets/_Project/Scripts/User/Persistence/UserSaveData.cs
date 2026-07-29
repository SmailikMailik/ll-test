using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LL.User.Persistence
{
    [JsonObject(MemberSerialization.OptIn, ItemRequired = Required.AllowNull)]
    internal sealed class UserSaveData
    {
        [JsonProperty] private int _version;
        [JsonProperty] private UserIdentitySaveData _identity;
        [JsonProperty] private UserProgressSaveData _progress;
        [JsonProperty] private UserPromotionOrderSaveData _promotionOrder;
        [JsonProperty] private ItemAmountSaveData[] _items;
        [JsonProperty] private string[] _claimedRewardIds;

        internal const int CurrentVersion = 2;

        internal int Version => _version;
        internal UserIdentitySaveData Identity => _identity;
        internal UserProgressSaveData Progress => _progress;
        internal UserPromotionOrderSaveData PromotionOrder => _promotionOrder;
        internal IReadOnlyList<ItemAmountSaveData> Items => _items;
        internal IReadOnlyList<string> ClaimedRewardIds => _claimedRewardIds;

        [JsonConstructor]
        private UserSaveData() { }

        internal UserSaveData(
            UserIdentitySaveData identity,
            UserProgressSaveData progress,
            UserPromotionOrderSaveData promotionOrder,
            ItemAmountSaveData[] items,
            string[] claimedRewardIds)
        {
            _version = CurrentVersion;
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));
            _progress = progress ?? throw new ArgumentNullException(nameof(progress));
            _promotionOrder = promotionOrder ?? throw new ArgumentNullException(nameof(promotionOrder));
            _items = items ?? Array.Empty<ItemAmountSaveData>();
            _claimedRewardIds = claimedRewardIds ?? Array.Empty<string>();
        }
    }

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

    [JsonObject(MemberSerialization.OptIn, ItemRequired = Required.AllowNull)]
    internal sealed class ItemAmountSaveData
    {
        [JsonProperty] private string _id;
        [JsonProperty] private int _amount;

        internal string Id => _id;
        internal int Amount => _amount;

        [JsonConstructor]
        private ItemAmountSaveData() { }

        internal ItemAmountSaveData(string id, int amount)
        {
            _id = id;
            _amount = amount;
        }
    }

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