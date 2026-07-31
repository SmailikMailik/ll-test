using System.Collections.Generic;
using LL.Infrastructure.Collections;

namespace LL.Game.Data.Declarations
{
    internal sealed class RewardDeclaration
    {
        internal string Id { get; }
        internal IReadOnlyList<RewardItemDeclaration> Items { get; }

        internal RewardDeclaration(string id, IEnumerable<RewardItemDeclaration> items)
        {
            Id = id;
            Items = items.ToReadOnlyCopy();
        }
    }
}