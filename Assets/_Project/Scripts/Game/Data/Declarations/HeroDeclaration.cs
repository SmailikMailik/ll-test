namespace LL.Game.Data.Declarations
{
    internal sealed class HeroDeclaration
    {
        internal string Id { get; }
        internal string NameLocalizationKey { get; }
        internal string CountryId { get; }

        internal HeroDeclaration(string id, string nameLocalizationKey, string countryId)
        {
            Id = id;
            NameLocalizationKey = nameLocalizationKey;
            CountryId = countryId;
        }
    }
}