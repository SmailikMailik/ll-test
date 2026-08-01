using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Heroes;
using LL.Game.Identifiers;

namespace LL.User.Snapshots
{
    internal sealed class UserHeroesSnapshot
    {
        internal IReadOnlyList<UserHeroSnapshot> Heroes { get; }

        private readonly IReadOnlyDictionary<HeroId, UserHeroSnapshot> _heroesById;

        internal UserHeroesSnapshot(IEnumerable<UserHeroSnapshot> heroes)
        {
            var heroArray = heroes?.ToArray() ?? Array.Empty<UserHeroSnapshot>();
            IdentifierCollectionValidator.EnsureValid(heroArray, hero => hero.HeroId, nameof(heroes));
            Heroes = Array.AsReadOnly(heroArray);
            _heroesById = heroArray.ToDictionary(hero => hero.HeroId);
        }

        internal bool TryGetHero(HeroId heroId, out UserHeroSnapshot hero) =>
            _heroesById.TryGetValue(heroId, out hero);
    }
}