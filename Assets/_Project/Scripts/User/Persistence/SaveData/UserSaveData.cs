using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LL.User.Persistence.SaveData
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
}