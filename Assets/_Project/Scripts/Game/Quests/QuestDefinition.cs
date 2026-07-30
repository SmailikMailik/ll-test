using System;

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
            if (string.IsNullOrWhiteSpace(id.Value))
                throw new ArgumentException("Quest ID must be non-empty.", nameof(id));

            if (string.IsNullOrWhiteSpace(titleLocalizationKey))
                throw new ArgumentException("Quest title localization key must be non-empty.");

            if (string.IsNullOrWhiteSpace(descriptionLocalizationKey))
                throw new ArgumentException("Quest description localization key must be non-empty.");

            Id = id;
            TitleLocalizationKey = titleLocalizationKey;
            DescriptionLocalizationKey = descriptionLocalizationKey;
        }
    }
}