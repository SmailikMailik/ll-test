using System;

namespace LL.User.Persistence.Documents
{
    internal sealed class UserSaveDocument
    {
        internal const int CurrentVersion = 1;

        public int Version { get; }
        public UserIdentityDocumentEntry Identity { get; }
        public UserProgressDocumentEntry Progress { get; }
        public UserPromotionQuestDocumentEntry PromotionQuest { get; }
        public ItemAmountDocumentEntry[] Items { get; }

        public UserSaveDocument(
            int version,
            UserIdentityDocumentEntry identity,
            UserProgressDocumentEntry progress,
            UserPromotionQuestDocumentEntry promotionQuest,
            ItemAmountDocumentEntry[] items)
        {
            Version = version;
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            Progress = progress ?? throw new ArgumentNullException(nameof(progress));
            PromotionQuest = promotionQuest ?? throw new ArgumentNullException(nameof(promotionQuest));
            Items = items ?? Array.Empty<ItemAmountDocumentEntry>();
        }
    }
}