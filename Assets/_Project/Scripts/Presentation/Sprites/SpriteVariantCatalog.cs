using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Identifiers;
using UnityEngine;

namespace LL.Presentation.Sprites
{
    internal sealed class SpriteVariantCatalog<TId, TVariant>
        where TId : struct, IIdentifier
        where TVariant : struct, Enum
    {
        private readonly IReadOnlyDictionary<(TId Id, TVariant Variant), Sprite> _sprites;

        internal SpriteVariantCatalog(IEnumerable<SpriteVariant<TId, TVariant>> sprites)
        {
            var copy = sprites?.ToArray() ?? Array.Empty<SpriteVariant<TId, TVariant>>();
            var spritesByKey = new Dictionary<(TId Id, TVariant Variant), Sprite>();

            foreach (var sprite in copy)
            {
                var key = (sprite.Id, sprite.Variant);

                if (spritesByKey.TryAdd(key, sprite.Sprite) is false)
                    throw new ArgumentException($"Duplicate sprite variant: {sprite.Id}, {sprite.Variant}.", nameof(sprites));
            }

            _sprites = spritesByKey;
        }

        internal Sprite GetSprite(TId id, TVariant variant)
        {
            if (_sprites.TryGetValue((id, variant), out var sprite))
                return sprite;

            throw new ArgumentException($"Unknown sprite variant: {id}, {variant}.", nameof(id));
        }
    }
}