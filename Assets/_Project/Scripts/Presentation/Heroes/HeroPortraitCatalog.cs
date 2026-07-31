using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Heroes;
using LL.Game.Identifiers;
using UnityEngine;

namespace LL.Presentation.Heroes
{
    internal sealed class HeroPortraitCatalog
    {
        private readonly IReadOnlyDictionary<HeroId, HeroPortraitDefinition> _portraitsByHeroId;

        internal HeroPortraitCatalog(IEnumerable<HeroPortraitDefinition> portraits)
        {
            var entries = portraits?.ToArray() ?? Array.Empty<HeroPortraitDefinition>();
            IdentifierCollectionValidator.EnsureValid(entries, entry => entry.HeroId, nameof(portraits));
            _portraitsByHeroId = entries.ToDictionary(entry => entry.HeroId);
        }

        internal Sprite GetPortrait(HeroId heroId, HeroPortraitSize size)
        {
            if (_portraitsByHeroId.TryGetValue(heroId, out var portrait))
                return portrait.GetPortrait(size);

            throw new ArgumentException($"Unknown hero portrait ID: {heroId}", nameof(heroId));
        }
    }
}