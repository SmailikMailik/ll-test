using System;

namespace LL.Game.Data.Persistence.Documents
{
    internal sealed class RankUpOptionDocumentEntry
    {
        public string OptionId { get; }
        public RankUpRequirementDocumentEntry[] Requirements { get; }

        public RankUpOptionDocumentEntry(
            string optionId,
            RankUpRequirementDocumentEntry[] requirements)
        {
            OptionId = optionId;
            Requirements = requirements ?? Array.Empty<RankUpRequirementDocumentEntry>();
        }
    }
}