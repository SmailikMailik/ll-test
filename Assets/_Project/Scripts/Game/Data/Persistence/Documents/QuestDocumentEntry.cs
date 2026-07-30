namespace LL.Game.Data.Persistence.Documents
{
    internal sealed class QuestDocumentEntry
    {
        public string Id { get; }
        public string TitleLocalizationKey { get; }
        public string DescriptionLocalizationKey { get; }

        public QuestDocumentEntry(
            string id,
            string titleLocalizationKey,
            string descriptionLocalizationKey)
        {
            Id = id;
            TitleLocalizationKey = titleLocalizationKey;
            DescriptionLocalizationKey = descriptionLocalizationKey;
        }
    }
}