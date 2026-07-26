using System;
using System.Linq;
using LL.Game.ExperienceCards;
using LL.Identifiers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Game.Configuration
{
    [CreateAssetMenu(fileName = nameof(ExperienceCardCatalogConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class ExperienceCardCatalogConfig : ScriptableObject, IDataLoader<ExperienceCardCatalog>
    {
        internal const string CreationPath = "LL/Game Data/Experience Card Catalog";

        [ValidateInput(nameof(HasValidCardIds), "Card IDs must be non-empty and unique.")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private ExperienceCardDefinitionEntry[] _cards;

        public ExperienceCardCatalog Load() => new
        (
            _cards?.Select(card => card?.ToCard())
        );

        private void OnValidate()
        {
            if (_cards == null)
                return;

            for (var index = 0; index < _cards.Length; index++)
            {
                var entry = _cards[index] ?? new ExperienceCardDefinitionEntry(string.Empty, 1);
                entry.Normalize();
                _cards[index] = entry;
            }
        }

        private static bool HasValidCardIds(ExperienceCardDefinitionEntry[] cards)
        {
            return IdentifierCatalogValidator.HasValidIds(cards, card => new ExperienceCardId(card.Id));
        }
    }

    [Serializable]
    internal sealed class ExperienceCardDefinitionEntry
    {
        [LabelText("ID")]
        [SerializeField] private string _id;

        [LabelText("Experience")]
        [SuffixLabel("XP", true)]
        [MinValue(1)]
        [SerializeField] private int _experienceAmount;

        internal string Id => _id;

        internal ExperienceCardDefinitionEntry(string id, int experienceAmount)
        {
            _id = id;
            _experienceAmount = experienceAmount;
        }

        internal IExperienceCard ToCard() => new ExperienceCard
        (
            new ExperienceCardId(_id),
            _experienceAmount
        );

        internal void Normalize()
        {
            _experienceAmount = Math.Max(1, _experienceAmount);
        }
    }
}