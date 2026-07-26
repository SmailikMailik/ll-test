using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Currencies;
using LL.Identifiers;
using LL.Presentation.Icons;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Presentation.Configuration
{
    [CreateAssetMenu(fileName = nameof(CurrencyIconCatalogConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class CurrencyIconCatalogConfig : ScriptableObject, IDataLoader<IconCatalog<CurrencyId>>
    {
        internal const string CreationPath = "LL/Presentation/Currency Icon Catalog";

        [ValidateInput(nameof(HasValidIconIds), "Currency icon IDs must be non-empty and unique.")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private CurrencyIconEntry[] _icons;

        public IconCatalog<CurrencyId> Load()
        {
            IdentifierCatalogValidator.EnsureValidIds(
                _icons,
                entry => entry.Id,
                nameof(CurrencyIconCatalogConfig),
                nameof(_icons));

            var icons = _icons?.Select(entry => entry.ToPair());

            return new IconCatalog<CurrencyId>(icons);
        }

        private static bool HasValidIconIds(CurrencyIconEntry[] icons)
        {
            return IdentifierCatalogValidator.HasValidIds(icons, entry => entry.Id);
        }
    }

    [Serializable]
    internal sealed class CurrencyIconEntry
    {
        [LabelText("Currency ID")]
        [SerializeField] private string _id;

        [Required]
        [PreviewField(55, ObjectFieldAlignment.Center)]
        [SerializeField] private Sprite _icon;

        internal CurrencyId Id => new(_id);

        internal KeyValuePair<CurrencyId, Sprite> ToPair() => new(Id, _icon);
    }
}