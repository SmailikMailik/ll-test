using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Identifiers;
using LL.Validation;

namespace LL.Game.Heroes
{
    internal sealed class HeroCatalog
    {
        private const string EntriesCode = "hero-catalog.entries.not-empty";

        internal IReadOnlyList<HeroDefinition> Heroes { get; }

        private readonly IReadOnlyDictionary<HeroId, HeroDefinition> _heroesById;

        internal HeroCatalog(IEnumerable<HeroDefinition> heroes)
        {
            var entries = heroes?.ToArray() ?? Array.Empty<HeroDefinition>();

            ValidationRunner.EnsureValid(
                context => ValidationRules.NotEmpty(entries, context, EntriesCode),
                nameof(heroes));
            IdentifierCollectionValidator.EnsureValid(entries, hero => hero.Id, nameof(heroes));

            Heroes = Array.AsReadOnly(entries);
            _heroesById = entries.ToDictionary(hero => hero.Id);
        }

        internal HeroDefinition GetHero(HeroId id)
        {
            if (_heroesById.TryGetValue(id, out var hero))
                return hero;

            throw new ArgumentException($"Unknown hero ID: {id}", nameof(id));
        }

        internal bool TryGetHero(HeroId id, out HeroDefinition hero) => _heroesById.TryGetValue(id, out hero);
    }
}