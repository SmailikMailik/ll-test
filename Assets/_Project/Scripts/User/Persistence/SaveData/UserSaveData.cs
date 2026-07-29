using System;

namespace LL.User.Persistence.SaveData
{
    internal sealed class UserSaveData
    {
        internal const int CurrentVersion = 3;

        public int Version { get; }
        public UserIdentitySaveData Identity { get; }
        public UserProgressSaveData Progress { get; }
        public UserPromotionOrderSaveData PromotionOrder { get; }
        public ItemAmountSaveData[] Items { get; }
        public string[] ClaimedRewardIds { get; }

        public UserSaveData(
            int version,
            UserIdentitySaveData identity,
            UserProgressSaveData progress,
            UserPromotionOrderSaveData promotionOrder,
            ItemAmountSaveData[] items,
            string[] claimedRewardIds)
        {
            Version = version;
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            Progress = progress ?? throw new ArgumentNullException(nameof(progress));
            PromotionOrder = promotionOrder ?? throw new ArgumentNullException(nameof(promotionOrder));
            Items = items ?? Array.Empty<ItemAmountSaveData>();
            ClaimedRewardIds = claimedRewardIds ?? Array.Empty<string>();
        }
    }
}