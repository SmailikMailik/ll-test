namespace LL.Game.Data.Persistence.Documents
{
    internal sealed class HeroDocumentEntry
    {
        public string Id { get; }
        public string NameLocalizationKey { get; }
        public string CountryId { get; }

        public HeroDocumentEntry(string id, string nameLocalizationKey, string countryId)
        {
            Id = id;
            NameLocalizationKey = nameLocalizationKey;
            CountryId = countryId;
        }
    }
}