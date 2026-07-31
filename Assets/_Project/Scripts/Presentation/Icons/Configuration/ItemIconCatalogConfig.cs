using System;
using System.Collections.Generic;
using LL.Game.Items;
using LL.Validation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Presentation.Icons.Configuration
{
    [CreateAssetMenu(fileName = nameof(ItemIconCatalogConfig), menuName = CreationPath)]
    internal sealed class ItemIconCatalogConfig : ScriptableObject, IValidationSource
    {
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [ValidateInput(nameof(HasValidIcons), "Item icon data is invalid.")]
        [SerializeField] private ItemIconEntry[] _icons;

        internal const string CreationPath = "LL/Presentation/Item Icon Catalog";

        private static readonly IDataValidator<ItemIconEntry[]> _validator = new ItemIconCatalogConfigValidator();

        internal IReadOnlyList<ItemIconEntry> Icons => _icons;

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
        [SerializeField] private string _id;

        [PreviewField(48, ObjectFieldAlignment.Center), TableColumnWidth(64)]
        [SerializeField, Required] private Sprite _icon;

        internal ItemId Id => new(_id);
        internal Sprite Icon => _icon;

        internal KeyValuePair<ItemId, Sprite> ToPair() => new(Id, Icon);
    }
}