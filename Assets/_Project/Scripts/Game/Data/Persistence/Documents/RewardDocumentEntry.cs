using System;

namespace LL.Game.Data.Persistence.Documents
{
    internal sealed class RewardDocumentEntry
    {
        public string Id { get; }
        public RewardItemDocumentEntry[] Items { get; }

        public RewardDocumentEntry(string id, RewardItemDocumentEntry[] items)
        {
            Id = id;
            Items = items ?? Array.Empty<RewardItemDocumentEntry>();
        }
    }
}