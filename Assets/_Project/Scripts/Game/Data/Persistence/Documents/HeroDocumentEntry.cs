namespace LL.Game.Data.Persistence.Documents
{
    internal sealed class HeroDocumentEntry
    {
        public string Id { get; }
        public string NameLocalizationKey { get; }
        public string FlagId { get; }

        public HeroDocumentEntry(string id, string nameLocalizationKey, string flagId)
        {
            Id = id;
            NameLocalizationKey = nameLocalizationKey;
            FlagId = flagId;
        }
    }
}