namespace LL.Game.Data.Declarations
{
    internal sealed class QuestDeclaration
    {
        internal string Id { get; }
        internal string TitleLocalizationKey { get; }
        internal string DescriptionLocalizationKey { get; }

        internal QuestDeclaration(string id, string titleLocalizationKey, string descriptionLocalizationKey)
        {
            Id = id;
            TitleLocalizationKey = titleLocalizationKey;
            DescriptionLocalizationKey = descriptionLocalizationKey;
        }
    }
}