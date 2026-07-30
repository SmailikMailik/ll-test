using LL.Game.Identifiers;
using LL.Validation;

namespace LL.Game.Quests.Configuration
{
    internal sealed class QuestCatalogConfigValidator : IDataValidator<QuestEntry[]>
    {
        private const string EntriesCode = "quest.entries.required";
        private const string TitleKeyCode = "quest.localization.title.not-empty";
        private const string DescriptionKeyCode = "quest.localization.description.not-empty";

        public void Validate(
            QuestEntry[] quests,
            ValidationContext context)
        {
            if (ValidationRules.NotNull(quests, context, EntriesCode) is false)
                return;

            IdentifierCollectionValidator.Validate(
                quests,
                quest => quest.Id,
                context);

            for (var index = 0; index < quests.Length; index++)
            {
                var quest = quests[index];

                if (quest == null)
                    continue;

                var questContext = context.At(index);
                ValidationRules.NotEmpty(
                    quest.TitleLocalizationKey,
                    questContext.At(nameof(QuestEntry.TitleLocalizationKey)),
                    TitleKeyCode);
                ValidationRules.NotEmpty(
                    quest.DescriptionLocalizationKey,
                    questContext.At(nameof(QuestEntry.DescriptionLocalizationKey)),
                    DescriptionKeyCode);
            }
        }
    }
}