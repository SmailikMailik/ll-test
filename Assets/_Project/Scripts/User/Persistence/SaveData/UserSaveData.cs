using System;

namespace LL.User.Persistence.SaveData
{
    internal sealed class UserSaveData
    {
        internal const int CurrentVersion = 6;

        public int Version { get; }
        public UserIdentitySaveData Identity { get; }
        public UserProgressSaveData Progress { get; }
        public UserPromotionQuestSaveData PromotionQuest { get; }
        public ItemAmountSaveData[] Items { get; }

        public UserSaveData(
            int version,
            UserIdentitySaveData identity,
            UserProgressSaveData progress,
            UserPromotionQuestSaveData promotionQuest,
            ItemAmountSaveData[] items)
        {
            Version = version;
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            Progress = progress ?? throw new ArgumentNullException(nameof(progress));
            PromotionQuest = promotionQuest ?? throw new ArgumentNullException(nameof(promotionQuest));
            Items = items ?? Array.Empty<ItemAmountSaveData>();
        }
    }
}