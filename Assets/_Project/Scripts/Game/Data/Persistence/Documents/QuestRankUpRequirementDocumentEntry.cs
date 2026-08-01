namespace LL.Game.Data.Persistence.Documents
{
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
}