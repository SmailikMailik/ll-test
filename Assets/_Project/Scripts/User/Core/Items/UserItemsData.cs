using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Items;

namespace LL.User.Core.Items
{
    internal sealed class UserItemsData
    {
        internal IReadOnlyList<ItemAmount> Amounts { get; }

        internal UserItemsData(IEnumerable<ItemAmount> entries)
        {
            var amounts = new Dictionary<ItemId, long>();

            if (entries != null)
            {
                foreach (var entry in entries)
                {
                    if (entry.Id.IsEmpty)
                        continue;

                    amounts.TryGetValue(entry.Id, out var current);
                    amounts[entry.Id] = Math.Min(current + entry.Amount, int.MaxValue);
                }
            }

            var copy = amounts.Select(pair => new ItemAmount(pair.Key, (int)pair.Value)).ToArray();
            Amounts = Array.AsReadOnly(copy);
        }
    }
}