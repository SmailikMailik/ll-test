using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Cards;
using LL.Identifiers;
using LL.Loading;
using LL.Presentation.Icons;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Presentation.Configuration
{
    [CreateAssetMenu(fileName = nameof(CardIconCatalogConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class CardIconCatalogConfig : ScriptableObject, IDataLoader<IconCatalog<CardId>>
    {
        internal const string CreationPath = "LL/Presentation/Card Icon Catalog";

        [ValidateInput(nameof(HasValidIconIds), "Card icon IDs must be non-empty and unique.")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private CardIconEntry[] _icons;

        public IconCatalog<CardId> Load()
        {
            IdentifierCatalogValidator.EnsureValidIds(
                _icons,
                entry => entry.Id,
                nameof(CardIconCatalogConfig),
                nameof(_icons));

            var icons = _icons?.Select(entry => entry.ToPair());

            return new IconCatalog<CardId>(icons);
        }

        private static bool HasValidIconIds(CardIconEntry[] icons)
        {
            return IdentifierCatalogValidator.HasValidIds(icons, entry => entry.Id);
        }
    }

    [Serializable]
    internal sealed class CardIconEntry
    {
        [LabelText("Card ID")]
        [SerializeField] private string _id;

        [Required]
        [PreviewField(80, ObjectFieldAlignment.Center)]
        [SerializeField] private Sprite _icon;

        internal CardId Id => new(_id);

        internal KeyValuePair<CardId, Sprite> ToPair() => new(Id, _icon);
    }
}