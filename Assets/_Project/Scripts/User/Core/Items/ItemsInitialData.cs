using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Items;

namespace LL.User.Core.Items
{
    internal sealed class ItemsInitialData
    {
        internal IReadOnlyList<ItemAmount> Amounts { get; }

        internal ItemsInitialData(IEnumerable<ItemAmount> items)
        {
            var amounts = new Dictionary<ItemId, long>();

            if (items != null)
            {
                foreach (var item in items)
                {
                    if (item == null || item.Id.IsEmpty)
                        continue;

                    amounts.TryGetValue(item.Id, out var current);
                    amounts[item.Id] = Math.Min(current + item.Amount, int.MaxValue);
                }
            }

            var copy = amounts.Select(pair => new ItemAmount(pair.Key, (int)pair.Value)).ToArray();
            Amounts = Array.AsReadOnly(copy);
        }
    }
}