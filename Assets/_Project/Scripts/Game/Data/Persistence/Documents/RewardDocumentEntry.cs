using System;

namespace LL.Game.Data.Persistence.Documents
{
    internal sealed class RewardDocumentEntry
    {
        public string Id { get; }
        public ItemAmountDocumentEntry[] Items { get; }

        public RewardDocumentEntry(string id, ItemAmountDocumentEntry[] items)
        {
            Id = id;
            Items = items ?? Array.Empty<ItemAmountDocumentEntry>();
        }
    }
}