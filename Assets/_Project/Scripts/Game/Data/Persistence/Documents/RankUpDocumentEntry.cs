using System;

namespace LL.Game.Data.Persistence.Documents
{
    internal sealed class RankUpDocumentEntry
    {
        public string RankId { get; }
        public string QuestId { get; }
        public string HeroLocalizationKey { get; }
        public int RequiredAmount { get; }
        public int DurationMinutes { get; }
        public PaymentDocumentEntry QuestPayment { get; }
        public PaymentDocumentEntry InstantPayment { get; }
        public string RewardId { get; }

        public RankUpDocumentEntry(
            string rankId,
            string questId,
            string heroLocalizationKey,
            int requiredAmount,
            int durationMinutes,
            PaymentDocumentEntry questPayment,
            PaymentDocumentEntry instantPayment,
            string rewardId)
        {
            RankId = rankId;
            QuestId = questId;
            HeroLocalizationKey = heroLocalizationKey;
            RequiredAmount = requiredAmount;
            DurationMinutes = durationMinutes;
            QuestPayment = questPayment ?? throw new ArgumentNullException(nameof(questPayment));
            InstantPayment = instantPayment ?? throw new ArgumentNullException(nameof(instantPayment));
            RewardId = rewardId;
        }
    }
}