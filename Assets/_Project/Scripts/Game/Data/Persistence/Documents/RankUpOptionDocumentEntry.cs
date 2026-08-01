using System;

namespace LL.Game.Data.Persistence.Documents
{
    internal sealed class RankUpOptionDocumentEntry
    {
        public string OptionId { get; }
        public QuestRankUpRequirementDocumentEntry[] Quests { get; }
        public PaymentRankUpRequirementDocumentEntry[] Payments { get; }

        public RankUpOptionDocumentEntry(
            string optionId,
            QuestRankUpRequirementDocumentEntry[] quests,
            PaymentRankUpRequirementDocumentEntry[] payments)
        {
            OptionId = optionId;
            Quests = quests ?? Array.Empty<QuestRankUpRequirementDocumentEntry>();
            Payments = payments ?? Array.Empty<PaymentRankUpRequirementDocumentEntry>();
        }
    }
}