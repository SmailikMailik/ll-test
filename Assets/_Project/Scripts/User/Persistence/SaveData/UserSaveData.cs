using System;

namespace LL.User.Persistence.SaveData
{
    internal sealed class UserSaveData
    {
        internal const int CurrentVersion = 4;

        public int Version { get; }
        public UserIdentitySaveData Identity { get; }
        public UserProgressSaveData Progress { get; }
        public UserPromotionOrderSaveData PromotionOrder { get; }
        public ItemAmountSaveData[] Items { get; }

        public UserSaveData(
            int version,
            UserIdentitySaveData identity,
            UserProgressSaveData progress,
            UserPromotionOrderSaveData promotionOrder,
            ItemAmountSaveData[] items)
        {
            Version = version;
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            Progress = progress ?? throw new ArgumentNullException(nameof(progress));
            PromotionOrder = promotionOrder ?? throw new ArgumentNullException(nameof(promotionOrder));
            Items = items ?? Array.Empty<ItemAmountSaveData>();
        }
    }
}