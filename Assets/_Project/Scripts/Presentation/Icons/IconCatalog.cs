using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Identifiers;
using UnityEngine;

namespace LL.Presentation.Icons
{
    internal sealed class IconCatalog<TId>
        where TId : struct, IIdentifier
    {
        private readonly IReadOnlyDictionary<TId, Sprite> _icons;

        internal IconCatalog(IEnumerable<KeyValuePair<TId, Sprite>> icons)
        {
            var copy = icons?.ToArray()
                ?? Array.Empty<KeyValuePair<TId, Sprite>>();

            IdentifierCollectionValidator.EnsureValid(
                copy,
                icon => icon.Key,
                nameof(icons));

            foreach (var icon in copy)
            {
                if (icon.Value == null)
                {
                    throw new ArgumentException(
                        $"Icon catalog contains a missing icon for ID: {icon.Key}.",
                        nameof(icons));
                }
            }

            _icons = copy.ToDictionary(icon => icon.Key, icon => icon.Value);
        }

        internal bool TryGetIcon(TId id, out Sprite icon)
        {
            return _icons.TryGetValue(id, out icon);
        }
    }
}