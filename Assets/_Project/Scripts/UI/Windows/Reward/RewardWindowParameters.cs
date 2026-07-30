using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Items;

namespace LL.UI.Windows.Reward
{
    internal sealed class RewardWindowParameters : IWindowParameters
    {
        internal int Rank { get; }
        internal IReadOnlyList<ItemAmount> Items { get; }

        internal RewardWindowParameters(int rank, IReadOnlyList<ItemAmount> items)
        {
            Rank = rank;
            Items = Array.AsReadOnly(items?.ToArray() ?? Array.Empty<ItemAmount>());
        }
    }
}