using System.Collections.Generic;
using LL.Game.Rewards;

namespace LL.Game.Rewards.Services
{
    internal interface IRewardGrantService
    {
        bool CanGrant(RewardBundleId id);
        bool TryGrant(RewardBundleId id, out IReadOnlyList<IReward> rewards);
    }
}