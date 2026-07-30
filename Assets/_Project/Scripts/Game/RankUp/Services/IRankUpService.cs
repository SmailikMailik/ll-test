using System.Collections.Generic;
using LL.Game.Items;
using LL.Game.Payments;

namespace LL.Game.RankUp.Services
{
    internal interface IRankUpService
    {
        bool TryGetDefinition(out RankUpDefinition definition);
        bool CanRankUp(Payment payment);
        bool TryRankUp(Payment payment, out IReadOnlyList<ItemAmount> rewardItems);
    }
}