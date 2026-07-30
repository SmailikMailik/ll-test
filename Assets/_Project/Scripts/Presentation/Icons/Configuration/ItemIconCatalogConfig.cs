using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Items;
using LL.Infrastructure.Loading;
using LL.Validation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Presentation.Icons.Configuration
{
    [CreateAssetMenu(fileName = nameof(ItemIconCatalogConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class ItemIconCatalogConfig :
        ScriptableObject,
        IDataLoader<IconCatalog<ItemId>>,
        IValidationSource
    {
        [ValidateInput(nameof(HasValidIcons), "Item icon data is invalid.")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private ItemIconEntry[] _icons;

        internal const string CreationPath = "LL/Presentation/Item Icon Catalog";

        private static readonly IDataValidator<ItemIconEntry[]> _validator =
            new ItemIconCatalogConfigValidator();

        internal IReadOnlyList<ItemIconEntry> Icons => _icons;

        public IconCatalog<ItemId> Load()
        {
            ValidationRunner.EnsureValid(this, nameof(_icons));

            var icons = _icons.Select(entry => entry.ToPair());

            return new IconCatalog<ItemId>(icons);
        }

        private static bool HasValidIcons(ItemIconEntry[] icons)
        {
            return ValidationRunner.IsValid(icons, _validator);
        }

        void IValidationSource.Validate(ValidationContext context)
        {
            _validator.Validate(_icons, context);
        }
    }

    [Serializable]
    internal sealed class ItemIconEntry
    {
        [LabelText("ID")]
        [SerializeField] private string _id;

        [SerializeField, Required, PreviewField(64, ObjectFieldAlignment.Center)] private Sprite _icon;

        internal ItemId Id => new(_id);
        internal Sprite Icon => _icon;

        internal KeyValuePair<ItemId, Sprite> ToPair() => new(Id, Icon);
    }
}