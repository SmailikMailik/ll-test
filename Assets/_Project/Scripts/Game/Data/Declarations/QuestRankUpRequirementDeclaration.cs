namespace LL.Game.Data.Declarations
{
    internal sealed class QuestRankUpRequirementDeclaration
    {
        internal string RequirementId { get; }
        internal string QuestId { get; }
        internal int RequiredCount { get; }
        internal int DurationMinutes { get; }

        internal QuestRankUpRequirementDeclaration(
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
}