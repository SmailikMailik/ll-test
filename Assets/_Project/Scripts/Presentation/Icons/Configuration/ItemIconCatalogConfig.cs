using System.Linq;
using System;
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

        internal ItemIconEntry[] Icons => _icons;

        public IconCatalog<ItemId> Load()
        {
            var result = ValidationRunner.Run(this);
            ValidationResultGuard.EnsureValid(result, nameof(_icons));

            var icons = _icons?.Select(entry => entry.ToPair());

            return new IconCatalog<ItemId>(icons);
        }

        private static bool HasValidIcons(ItemIconEntry[] icons)
        {
            return ValidationRunner.Run(icons, _validator).IsValid;
        }

        void IValidationSource.Validate(ValidationContext context)
        {
            _validator.Validate(_icons, context);
        }
    }

    [Serializable]
    internal sealed class ItemIconEntry : IconEntry<ItemId>
    {
        protected override ItemId CreateId(string value) => new(value);
    }
}