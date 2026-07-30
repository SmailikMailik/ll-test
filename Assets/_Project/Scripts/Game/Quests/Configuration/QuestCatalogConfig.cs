using System;
using System.Collections.Generic;
using System.Linq;
using LL.Infrastructure.Loading;
using LL.Validation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Game.Quests.Configuration
{
    [CreateAssetMenu(fileName = nameof(QuestCatalogConfig), menuName = CreationPath)]
    internal sealed class QuestCatalogConfig :
        ScriptableObject,
        IDataLoader<QuestCatalog>,
        IValidationSource
    {
        [ValidateInput(nameof(HasValidQuests), "Quest data is invalid.")]
        [SerializeField] private QuestEntry[] _quests;

        internal const string CreationPath = "LL/Game Data/Quest Catalog";

        private static readonly IDataValidator<QuestEntry[]> _validator =
            new QuestCatalogConfigValidator();

        internal IReadOnlyList<QuestEntry> Quests => _quests;

        public QuestCatalog Load()
        {
            ValidationRunner.EnsureValid(this, nameof(_quests));

            return new QuestCatalog(_quests.Select(entry => entry.ToQuest()));
        }

        private static bool HasValidQuests(QuestEntry[] quests)
        {
            return ValidationRunner.IsValid(quests, _validator);
        }

        void IValidationSource.Validate(ValidationContext context)
        {
            _validator.Validate(_quests, context);
        }
    }

    [Serializable]
    internal sealed class QuestEntry
    {
        [SerializeField] private string _id;

        [SerializeField] private string _titleLocalizationKey;

        [SerializeField] private string _descriptionLocalizationKey;

        internal QuestId Id => new(_id);
        internal string TitleLocalizationKey => _titleLocalizationKey;
        internal string DescriptionLocalizationKey => _descriptionLocalizationKey;

        internal QuestDefinition ToQuest() =>
            new(Id, TitleLocalizationKey, DescriptionLocalizationKey);
    }
}