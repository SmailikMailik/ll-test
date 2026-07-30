using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Identifiers;
using LL.Game.Ranks;

namespace LL.Game.RankUp
{
    internal sealed class RankUpCatalog
    {
        internal IReadOnlyList<RankUpDefinition> Definitions { get; }

        private readonly IReadOnlyDictionary<RankId, RankUpDefinition> _definitionsByRankId;

        internal RankUpCatalog(IEnumerable<RankUpDefinition> definitions)
        {
            var definitionArray = definitions?.ToArray() ?? Array.Empty<RankUpDefinition>();
            IdentifierCollectionValidator.EnsureValid(
                definitionArray,
                definition => definition.RankId,
                nameof(definitions));

            Definitions = Array.AsReadOnly(definitionArray);
            _definitionsByRankId = definitionArray.ToDictionary(definition => definition.RankId);
        }

        internal bool TryGetDefinition(RankId rankId, out RankUpDefinition definition) =>
            _definitionsByRankId.TryGetValue(rankId, out definition);
    }
}