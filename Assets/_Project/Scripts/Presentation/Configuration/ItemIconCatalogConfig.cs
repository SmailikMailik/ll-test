using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Items;
using LL.Identifiers;
using LL.Loading;
using LL.Presentation.Icons;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Presentation.Configuration
{
    [CreateAssetMenu(fileName = nameof(ItemIconCatalogConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class ItemIconCatalogConfig : ScriptableObject, IDataLoader<IconCatalog<ItemId>>
    {
        internal const string CreationPath = "LL/Presentation/Item Icon Catalog";

        [ValidateInput(nameof(HasValidIconIds), "Item icon IDs must be non-empty and unique.")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private ItemIconEntry[] _icons;

        public IconCatalog<ItemId> Load()
        {
            IdentifierCatalogValidator.EnsureValidIds(
                _icons,
                entry => entry.Id,
                nameof(ItemIconCatalogConfig),
                nameof(_icons));

            var icons = _icons?.Select(entry => entry.ToPair());

            return new IconCatalog<ItemId>(icons);
        }

        private static bool HasValidIconIds(ItemIconEntry[] icons)
        {
            return IdentifierCatalogValidator.HasValidIds(icons, entry => entry.Id);
        }
    }

    [Serializable]
    internal sealed class ItemIconEntry
    {
        [LabelText("Item ID")]
        [SerializeField] private string _id;

        [Required]
        [PreviewField(80, ObjectFieldAlignment.Center)]
        [SerializeField] private Sprite _icon;

        internal ItemId Id => new(_id);

        internal KeyValuePair<ItemId, Sprite> ToPair() => new(Id, _icon);
    }
}