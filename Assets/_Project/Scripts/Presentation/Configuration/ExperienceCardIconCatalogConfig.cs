using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.ExperienceCards;
using LL.Presentation.Icons;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Presentation.Configuration
{
    [CreateAssetMenu(fileName = nameof(ExperienceCardIconCatalogConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class ExperienceCardIconCatalogConfig :
        ScriptableObject,
        IDataLoader<IconCatalog<ExperienceCardId>>
    {
        internal const string CreationPath = "LL/Presentation/Experience Card Icon Catalog";

        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private ExperienceCardIconEntry[] _icons;

        public IconCatalog<ExperienceCardId> Load()
        {
            var icons = _icons?
                .Where(entry => entry != null && entry.Id.IsEmpty is false)
                .Select(entry => entry.ToPair());

            return new IconCatalog<ExperienceCardId>(icons);
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

        internal KeyValuePair<ExperienceCardId, Sprite> ToPair()
        {
            return new KeyValuePair<ExperienceCardId, Sprite>(Id, _icon);
        }
    }
}