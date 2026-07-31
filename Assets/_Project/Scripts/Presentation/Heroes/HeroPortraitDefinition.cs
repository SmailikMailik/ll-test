using System;
using LL.Game.Heroes;
using LL.Game.Identifiers;
using UnityEngine;

namespace LL.Presentation.Heroes
{
    internal sealed class HeroPortraitDefinition
    {
        internal HeroId HeroId { get; }
        internal Sprite SmallPortrait { get; }
        internal Sprite LargePortrait { get; }

        internal HeroPortraitDefinition(HeroId heroId, Sprite smallPortrait, Sprite largePortrait)
        {
            IdentifierValidator.EnsureValid(heroId, nameof(heroId));
            HeroId = heroId;
            SmallPortrait = smallPortrait != null ? smallPortrait : throw new ArgumentNullException(nameof(smallPortrait));
            LargePortrait = largePortrait != null ? largePortrait : throw new ArgumentNullException(nameof(largePortrait));
        }

        internal Sprite GetPortrait(HeroPortraitSize size) => size switch
        {
            HeroPortraitSize.Small => SmallPortrait,
            HeroPortraitSize.Large => LargePortrait,
            _ => throw new ArgumentOutOfRangeException(nameof(size), size, "Unknown hero portrait size.")
        };
    }
}