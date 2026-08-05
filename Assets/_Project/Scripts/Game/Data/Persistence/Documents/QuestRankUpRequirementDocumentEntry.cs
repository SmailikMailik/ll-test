namespace LL.Game.Data.Persistence.Documents
{
    internal sealed class QuestRankUpRequirementDocumentEntry : RankUpRequirementDocumentEntry
    {
        public string QuestId { get; }
        public int RequiredCount { get; }
        public int DurationMinutes { get; }

        public QuestRankUpRequirementDocumentEntry(
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