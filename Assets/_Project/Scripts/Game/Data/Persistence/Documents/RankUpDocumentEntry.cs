using System;

namespace LL.Game.Data.Persistence.Documents
{
    internal sealed class RankUpDocumentEntry
    {
        public string HeroId { get; }
        public string RankId { get; }
        public string RewardId { get; }
        public RankUpOptionDocumentEntry[] Options { get; }

        public RankUpDocumentEntry(
            string heroId,
            string rankId,
            string rewardId,
            RankUpOptionDocumentEntry[] options)
        {
            HeroId = heroId;
            RankId = rankId;
            RewardId = rewardId;
            Options = options ?? Array.Empty<RankUpOptionDocumentEntry>();
        }
    }

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

    internal sealed class QuestRankUpRequirementDocumentEntry
    {
        public string RequirementId { get; }
        public string QuestId { get; }
        public int RequiredCount { get; }
        public int DurationMinutes { get; }

        public QuestRankUpRequirementDocumentEntry(
            string requirementId,
            string questId,
            int requiredCount,
            int durationMinutes)
        {
            RequirementId = requirementId;
            QuestId = questId;
            RequiredCount = requiredCount;
            DurationMinutes = durationMinutes;
        }
    }

    internal sealed class PaymentRankUpRequirementDocumentEntry
    {
        public string RequirementId { get; }
        public PaymentDocumentEntry Payment { get; }

        public PaymentRankUpRequirementDocumentEntry(
            string requirementId,
            PaymentDocumentEntry payment)
        {
            RequirementId = requirementId;
            Payment = payment ?? throw new ArgumentNullException(nameof(payment));
        }
    }
}