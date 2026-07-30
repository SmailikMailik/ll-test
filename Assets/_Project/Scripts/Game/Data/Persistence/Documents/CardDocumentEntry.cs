namespace LL.Game.Data.Persistence.Documents
{
    internal sealed class CardDocumentEntry
    {
        public string Id { get; }
        public int ExperienceAmount { get; }

        public CardDocumentEntry(string id, int experienceAmount)
        {
            Id = id;
            ExperienceAmount = experienceAmount;
        }
    }
}