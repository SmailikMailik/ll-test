namespace LL.Game.Data.Declarations
{
    internal sealed class QuestRankUpRequirementDeclaration : RankUpRequirementDeclaration
    {
        internal string QuestId { get; }
        internal int RequiredCount { get; }
        internal int DurationMinutes { get; }

        internal QuestRankUpRequirementDeclaration(
            string requirementId,
            string questId,
            int requiredCount,
            int durationMinutes) : base(requirementId)
        {
            QuestId = questId;
            RequiredCount = requiredCount;
            DurationMinutes = durationMinutes;
        }
    }
}