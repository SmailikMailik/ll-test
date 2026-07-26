using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Cards;

namespace LL.User.Core.Cards
{
    internal sealed class CardsInitialData
    {
        internal IReadOnlyList<CardStack> Stacks { get; }

        internal CardsInitialData(IEnumerable<CardStack> stacks)
        {
            var amounts = new Dictionary<CardId, long>();

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

            var copy = amounts.Select(pair => new CardStack(pair.Key, (int)pair.Value)).ToArray();
            Stacks = Array.AsReadOnly(copy);
        }
    }
}