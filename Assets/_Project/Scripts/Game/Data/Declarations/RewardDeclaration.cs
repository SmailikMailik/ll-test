using System;
using System.Collections.Generic;

namespace LL.Game.Data.Declarations
{
    internal sealed class RewardDeclaration
    {
        internal string Id { get; }
        internal IReadOnlyList<ItemAmountDeclaration> Items { get; }

        internal RewardDeclaration(string id, IEnumerable<ItemAmountDeclaration> items)
        {
            Id = id;
            var copy = items == null
                ? Array.Empty<ItemAmountDeclaration>()
                : new List<ItemAmountDeclaration>(items).ToArray();

            Items = Array.AsReadOnly(copy);
        }
    }
}