using System.Collections.Generic;
using LL.Game.Items;

namespace LL.Game.Rewards.Services
{
    internal interface IRewardGrantService
    {
        bool CanGrant(RewardId id);
        bool TryGrant(RewardId id, out IReadOnlyList<ItemAmount> items);
    }
}