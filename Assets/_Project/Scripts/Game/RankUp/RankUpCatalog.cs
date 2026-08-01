using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Heroes;
using LL.Game.Ranks;

namespace LL.Game.RankUp
{
    internal sealed class RankUpCatalog
    {
        internal IReadOnlyList<RankUpDefinition> Definitions { get; }

        private readonly IReadOnlyDictionary<RankUpKey, RankUpDefinition> _definitionsByKey;

        internal RankUpCatalog(IEnumerable<RankUpDefinition> definitions)
        {
            var definitionArray = definitions?.ToArray() ?? Array.Empty<RankUpDefinition>();

            if (definitionArray.Any(definition => definition is null))
                throw new ArgumentException("Rank-up definitions must not contain null entries.", nameof(definitions));

            var keys = definitionArray.Select(definition => new RankUpKey(definition.HeroId, definition.RankId)).ToArray();

            if (keys.Distinct().Count() != keys.Length)
                throw new ArgumentException("Hero and rank pairs must be unique.", nameof(definitions));

            Definitions = Array.AsReadOnly(definitionArray);
            _definitionsByKey = definitionArray.ToDictionary(
                definition => new RankUpKey(definition.HeroId, definition.RankId));
        }

        internal bool TryGetDefinition(
            HeroId heroId,
            RankId rankId,
            out RankUpDefinition definition) =>
            _definitionsByKey.TryGetValue(new RankUpKey(heroId, rankId), out definition);

        private readonly struct RankUpKey : IEquatable<RankUpKey>
        {
            private readonly HeroId _heroId;
            private readonly RankId _rankId;

            internal RankUpKey(HeroId heroId, RankId rankId)
            {
                _heroId = heroId;
                _rankId = rankId;
            }

            public bool Equals(RankUpKey other) =>
                _heroId.Equals(other._heroId) &&
                _rankId.Equals(other._rankId);

            public override bool Equals(object obj) => obj is RankUpKey other && Equals(other);
            public override int GetHashCode() => HashCode.Combine(_heroId, _rankId);
        }
    }
}