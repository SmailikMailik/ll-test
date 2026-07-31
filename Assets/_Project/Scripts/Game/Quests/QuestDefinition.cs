using System;
using LL.Game.Identifiers;
using LL.Validation;

namespace LL.Game.Quests
{
    internal sealed class QuestDefinition
    {
        internal QuestId Id { get; }
        internal string TitleLocalizationKey { get; }
        internal string DescriptionLocalizationKey { get; }

        internal QuestDefinition(
            QuestId id,
            string titleLocalizationKey,
            string descriptionLocalizationKey)
        {
            IdentifierValidator.EnsureValid(id, nameof(id));

            if (ValidationChecks.IsEmpty(titleLocalizationKey))
                throw new ArgumentException("Quest title localization key must be non-empty.");

            if (ValidationChecks.IsEmpty(descriptionLocalizationKey))
                throw new ArgumentException("Quest description localization key must be non-empty.");

            Id = id;
            TitleLocalizationKey = titleLocalizationKey;
            DescriptionLocalizationKey = descriptionLocalizationKey;
        }
    }
}