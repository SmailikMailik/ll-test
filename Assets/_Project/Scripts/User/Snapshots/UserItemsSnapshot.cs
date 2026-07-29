using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Identifiers;
using LL.Game.Items;

namespace LL.User.Snapshots
{
    internal sealed class UserItemsSnapshot
    {
        internal IReadOnlyList<ItemAmount> Amounts { get; }

        internal UserItemsSnapshot(IEnumerable<ItemAmount> entries)
        {
            var copy = entries?.ToArray() ?? Array.Empty<ItemAmount>();

            IdentifierCollectionValidator.EnsureValid(
                copy,
                entry => entry.Id,
                nameof(entries));

            Amounts = Array.AsReadOnly(copy);
        }
    }
}