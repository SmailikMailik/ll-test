using System.Collections.Generic;
using LL.Rewards.Models;

namespace LL.Rewards.Services
{
    internal interface IRewardGrantService
    {
        bool TryGrant(RewardBundleId id, out IReadOnlyList<IReward> rewards);
    }
}