using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Items;

namespace LL.UI.Windows.Reward
{
    internal sealed class RewardWindowParameters : IWindowParameters
    {
        internal int RankNumber { get; }
        internal IReadOnlyList<ItemAmount> Items { get; }

        internal RewardWindowParameters(int rankNumber, IReadOnlyList<ItemAmount> items)
        {
            RankNumber = rankNumber;
            Items = Array.AsReadOnly(items?.ToArray() ?? Array.Empty<ItemAmount>());
        }
    }
}