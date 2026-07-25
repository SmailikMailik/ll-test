using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.ExperienceCards;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Game.Configuration
{
    [CreateAssetMenu(fileName = nameof(ExperienceCardCatalogConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class ExperienceCardCatalogConfig : ScriptableObject, IDataLoader<ExperienceCardCatalogData>
    {
        internal const string CreationPath = "LL/Game Data/Experience Card Catalog";

        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private ExperienceCardDefinitionEntry[] _cards;

        public ExperienceCardCatalogData Load() => new
        (
            _cards?.Select(card => card?.ToData())
        );

        private void OnValidate()
        {
            if (_cards == null)
                return;

            var usedIds = new HashSet<string>(StringComparer.Ordinal);

            for (var index = 0; index < _cards.Length; index++)
            {
                var fallbackId = $"card_{index + 1}";
                var entry = _cards[index] ?? new ExperienceCardDefinitionEntry(
                    fallbackId,
                    1,
                    0);

                var baseId = new ExperienceCardId(entry.Id);

                if (baseId.IsEmpty)
                    baseId = new ExperienceCardId(fallbackId);

                var uniqueId = baseId.Value;
                var suffix = 2;

                while (usedIds.Add(uniqueId) is false)
                    uniqueId = $"{baseId.Value}_{suffix++}";

                entry.Normalize(uniqueId);
                _cards[index] = entry;
            }
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

        [LabelText("Capacity")]
        [MinValue(0)]
        [SerializeField] private int _capacity;

        internal string Id => _id;

        internal ExperienceCardDefinitionEntry(
            string id,
            int experienceAmount,
            int capacity)
        {
            _id = id;
            _experienceAmount = experienceAmount;
            _capacity = capacity;
        }

        internal ExperienceCardDefinitionData ToData()
        {
            return new ExperienceCardDefinitionData(
                new ExperienceCardId(_id),
                _experienceAmount,
                _capacity);
        }

        internal void Normalize(string id)
        {
            _id = id;
            _experienceAmount = Math.Max(1, _experienceAmount);
            _capacity = Math.Max(0, _capacity);
        }
    }
}