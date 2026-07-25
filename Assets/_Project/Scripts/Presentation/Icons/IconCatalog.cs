using System.Collections.Generic;
using UnityEngine;

namespace LL.Presentation.Icons
{
    internal sealed class IconCatalog<TId> : IIconProvider<TId> where TId : struct
    {
        private readonly IReadOnlyDictionary<TId, Sprite> _icons;

        internal IconCatalog(IEnumerable<KeyValuePair<TId, Sprite>> icons)
        {
            var uniqueIcons = new Dictionary<TId, Sprite>();

            if (icons != null)
            {
                foreach (var icon in icons)
                {
                    if (icon.Value == null || uniqueIcons.ContainsKey(icon.Key))
                        continue;

                    uniqueIcons.Add(icon.Key, icon.Value);
                }
            }

            _icons = uniqueIcons;
        }

        public bool TryGetIcon(TId id, out Sprite icon)
        {
            return _icons.TryGetValue(id, out icon);
        }
    }
}