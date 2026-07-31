using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Identifiers;
using LL.Validation;

namespace LL.Game.Ranks
{
    internal sealed class RankCatalog
    {
        private const string EntriesCode = "rank-catalog.entries.not-empty";

        internal IReadOnlyList<RankDefinition> Ranks { get; }

        private readonly IReadOnlyDictionary<RankId, RankDefinition> _ranksById;

        internal RankCatalog(IEnumerable<RankDefinition> ranks)
        {
            var entries = ranks?.ToArray() ?? Array.Empty<RankDefinition>();

            ValidationRunner.EnsureValid(
                context => ValidationRules.NotEmpty(
                    entries,
                    context,
                    EntriesCode),
                nameof(ranks));

            IdentifierCollectionValidator.EnsureValid(
                entries,
                rank => rank.Id,
                nameof(ranks));

            Ranks = Array.AsReadOnly(entries);
            _ranksById = entries.ToDictionary(rank => rank.Id);
        }

        internal RankDefinition GetRank(RankId id)
        {
            if (_ranksById.TryGetValue(id, out var rank))
                return rank;

            throw new ArgumentException($"Unknown rank ID: {id}", nameof(id));
        }

        internal bool TryGetRank(RankId id, out RankDefinition rank) =>
            _ranksById.TryGetValue(id, out rank);
    }
}