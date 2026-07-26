using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.ExperienceCards;
using LL.Identifiers;
using LL.Presentation.Icons;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Presentation.Configuration
{
    [CreateAssetMenu(fileName = nameof(ExperienceCardIconCatalogConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class ExperienceCardIconCatalogConfig : ScriptableObject, IDataLoader<IconCatalog<ExperienceCardId>>
    {
        internal const string CreationPath = "LL/Presentation/Experience Card Icon Catalog";

        [ValidateInput(nameof(HasValidIconIds), "Experience card icon IDs must be non-empty and unique.")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private ExperienceCardIconEntry[] _icons;

        public IconCatalog<ExperienceCardId> Load()
        {
            IdentifierCatalogValidator.EnsureValidIds(
                _icons,
                entry => entry.Id,
                nameof(ExperienceCardIconCatalogConfig),
                nameof(_icons));

            var icons = _icons?.Select(entry => entry.ToPair());

            return new IconCatalog<ExperienceCardId>(icons);
        }

        private static bool HasValidIconIds(ExperienceCardIconEntry[] icons)
        {
            return IdentifierCatalogValidator.HasValidIds(icons, entry => entry.Id);
        }
    }

    [Serializable]
    internal sealed class ExperienceCardIconEntry
    {
        [LabelText("Card ID")]
        [SerializeField] private string _id;

        [Required]
        [PreviewField(80, ObjectFieldAlignment.Center)]
        [SerializeField] private Sprite _icon;

        internal ExperienceCardId Id => new(_id);

        internal KeyValuePair<ExperienceCardId, Sprite> ToPair() => new(Id, _icon);
    }
}