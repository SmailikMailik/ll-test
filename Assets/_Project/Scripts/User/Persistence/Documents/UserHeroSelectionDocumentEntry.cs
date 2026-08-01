namespace LL.User.Persistence.Documents
{
    internal sealed class UserHeroSelectionDocumentEntry
    {
        public string HeroId { get; }

        public UserHeroSelectionDocumentEntry(string heroId)
        {
            HeroId = heroId;
        }
    }
}