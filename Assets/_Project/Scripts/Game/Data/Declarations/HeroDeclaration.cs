namespace LL.Game.Data.Declarations
{
    internal sealed class HeroDeclaration
    {
        internal string Id { get; }
        internal string NameLocalizationKey { get; }
        internal string FlagId { get; }

        internal HeroDeclaration(string id, string nameLocalizationKey, string flagId)
        {
            Id = id;
            NameLocalizationKey = nameLocalizationKey;
            FlagId = flagId;
        }
    }
}