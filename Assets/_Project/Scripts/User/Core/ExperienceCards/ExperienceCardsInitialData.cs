using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.ExperienceCards;

namespace LL.User.Core.ExperienceCards
{
    internal sealed class ExperienceCardsInitialData
    {
        internal IReadOnlyList<ExperienceCardStack> Stacks { get; }

        internal ExperienceCardsInitialData(
            IEnumerable<ExperienceCardStack> stacks)
        {
            var amounts = new Dictionary<ExperienceCardId, long>();

            if (stacks != null)
            {
                foreach (var stack in stacks)
                {
                    if (stack == null || stack.Id.IsEmpty)
                        continue;

                    amounts.TryGetValue(stack.Id, out var current);
                    amounts[stack.Id] = Math.Min(
                        current + stack.Amount,
                        int.MaxValue);
                }
            }

            var copy = amounts
                .Select(pair => new ExperienceCardStack(
                    pair.Key,
                    (int)pair.Value))
                .ToArray();

            Stacks = Array.AsReadOnly(copy);
        }
    }
}