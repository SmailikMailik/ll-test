namespace LL.Game.Data.Declarations
{
    internal sealed class RankUpDeclaration
    {
        internal string RankId { get; }
        internal string QuestId { get; }
        internal string HeroId { get; }
        internal int RequiredCount { get; }
        internal int DurationMinutes { get; }
        internal PaymentDeclaration QuestPayment { get; }
        internal PaymentDeclaration InstantPayment { get; }
        internal string RewardId { get; }

        internal RankUpDeclaration(
            string rankId,
            string questId,
            string heroId,
            int requiredCount,
            int durationMinutes,
            PaymentDeclaration questPayment,
            PaymentDeclaration instantPayment,
            string rewardId)
        {
            RankId = rankId;
            QuestId = questId;
            HeroId = heroId;
            RequiredCount = requiredCount;
            DurationMinutes = durationMinutes;
            QuestPayment = questPayment;
            InstantPayment = instantPayment;
            RewardId = rewardId;
        }
    }
}