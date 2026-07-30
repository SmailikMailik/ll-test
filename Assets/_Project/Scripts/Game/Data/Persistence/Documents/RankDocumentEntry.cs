namespace LL.Game.Data.Persistence.Documents
{
    internal sealed class RankDocumentEntry
    {
        public string Id { get; }
        public int RequiredExperience { get; }

        public RankDocumentEntry(string id, int requiredExperience)
        {
            Id = id;
            RequiredExperience = requiredExperience;
        }
    }
}