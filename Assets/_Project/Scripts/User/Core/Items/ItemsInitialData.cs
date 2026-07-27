using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Items;

namespace LL.User.Core.Items
{
    internal sealed class ItemsInitialData
    {
        internal IReadOnlyList<ItemStack> Stacks { get; }

        internal ItemsInitialData(IEnumerable<ItemStack> stacks)
        {
            var amounts = new Dictionary<ItemId, long>();

            if (stacks != null)
            {
                foreach (var stack in stacks)
                {
                    if (stack == null || stack.Id.IsEmpty)
                        continue;

                    amounts.TryGetValue(stack.Id, out var current);
                    amounts[stack.Id] = Math.Min(current + stack.Amount, int.MaxValue);
                }
            }

            var copy = amounts.Select(pair => new ItemStack(pair.Key, (int)pair.Value)).ToArray();
            Stacks = Array.AsReadOnly(copy);
        }
    }
}