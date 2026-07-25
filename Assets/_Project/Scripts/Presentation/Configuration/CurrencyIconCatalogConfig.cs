using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Currencies;
using LL.Presentation.Icons;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Presentation.Configuration
{
    [CreateAssetMenu(fileName = nameof(CurrencyIconCatalogConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class CurrencyIconCatalogConfig :
        ScriptableObject,
        IDataLoader<IconCatalog<CurrencyId>>
    {
        internal const string CreationPath = "LL/Presentation/Currency Icon Catalog";

        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private CurrencyIconEntry[] _icons;

        public IconCatalog<CurrencyId> Load()
        {
            var icons = _icons?
                .Where(entry => entry != null && entry.Id.IsEmpty is false)
                .Select(entry => entry.ToPair());

            return new IconCatalog<CurrencyId>(icons);
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

        internal KeyValuePair<CurrencyId, Sprite> ToPair()
        {
            return new KeyValuePair<CurrencyId, Sprite>(Id, _icon);
        }
    }
}