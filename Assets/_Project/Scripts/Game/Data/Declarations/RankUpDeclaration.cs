namespace LL.Game.Data.Declarations
{
    internal sealed class RankUpDeclaration
    {
        internal string RankId { get; }
        internal string QuestId { get; }
        internal string HeroId { get; }
        internal int RequiredAmount { get; }
        internal int DurationMinutes { get; }
        internal PaymentDeclaration QuestPayment { get; }
        internal PaymentDeclaration InstantPayment { get; }
        internal string RewardId { get; }

        internal RankUpDeclaration(
            string rankId,
            string questId,
            string heroId,
            int requiredAmount,
            int durationMinutes,
            PaymentDeclaration questPayment,
            PaymentDeclaration instantPayment,
            string rewardId)
        {
            RankId = rankId;
            QuestId = questId;
            HeroId = heroId;
            RequiredAmount = requiredAmount;
            DurationMinutes = durationMinutes;
            QuestPayment = questPayment;
            InstantPayment = instantPayment;
            RewardId = rewardId;
        }
    }
}