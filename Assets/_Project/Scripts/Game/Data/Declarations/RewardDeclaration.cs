using System;
using System.Collections.Generic;

namespace LL.Game.Data.Declarations
{
    internal sealed class RewardDeclaration
    {
        internal string Id { get; }
        internal IReadOnlyList<RewardItemDeclaration> Items { get; }

        internal RewardDeclaration(string id, IEnumerable<RewardItemDeclaration> items)
        {
            Id = id;
            var copy = items == null
                ? Array.Empty<RewardItemDeclaration>()
                : new List<RewardItemDeclaration>(items).ToArray();

            Items = Array.AsReadOnly(copy);
        }
    }
}