using System;
using LL.Game.Identifiers;
using UnityEngine;

namespace LL.Presentation.Sprites
{
    internal readonly struct SpriteVariant<TId, TVariant>
        where TId : struct, IIdentifier
        where TVariant : struct, Enum
    {
        internal TId Id { get; }
        internal TVariant Variant { get; }
        internal Sprite Sprite { get; }

        internal SpriteVariant(TId id, TVariant variant, Sprite sprite)
        {
            IdentifierValidator.EnsureValid(id, nameof(id));

            if (Enum.IsDefined(typeof(TVariant), variant) is false)
                throw new ArgumentOutOfRangeException(nameof(variant), variant, "Unknown sprite variant.");

            Id = id;
            Variant = variant;
            Sprite = sprite != null ? sprite : throw new ArgumentNullException(nameof(sprite));
        }
    }
}