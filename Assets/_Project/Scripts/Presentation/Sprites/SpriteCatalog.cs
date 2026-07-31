using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Identifiers;
using UnityEngine;

namespace LL.Presentation.Sprites
{
    internal sealed class SpriteCatalog<TId>
        where TId : struct, IIdentifier
    {
        private readonly IReadOnlyDictionary<TId, Sprite> _sprites;

        internal SpriteCatalog(IEnumerable<KeyValuePair<TId, Sprite>> sprites)
        {
            var copy = sprites?.ToArray() ?? Array.Empty<KeyValuePair<TId, Sprite>>();
            IdentifierCollectionValidator.EnsureValid(copy, sprite => sprite.Key, nameof(sprites));

            foreach (var sprite in copy)
            {
                if (sprite.Value == null)
                    throw new ArgumentException($"Sprite catalog contains a missing sprite for ID: {sprite.Key}.", nameof(sprites));
            }

            _sprites = copy.ToDictionary(sprite => sprite.Key, sprite => sprite.Value);
        }

        internal bool TryGetSprite(TId id, out Sprite sprite)
        {
            return _sprites.TryGetValue(id, out sprite);
        }
    }
}