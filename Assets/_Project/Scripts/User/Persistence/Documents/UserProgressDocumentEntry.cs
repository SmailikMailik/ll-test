namespace LL.User.Persistence.Documents
{
    internal sealed class UserProgressDocumentEntry
    {
        public string RankId { get; }
        public int Experience { get; }

        public UserProgressDocumentEntry(string rankId, int experience)
        {
            RankId = rankId;
            Experience = experience;
        }
    }
}