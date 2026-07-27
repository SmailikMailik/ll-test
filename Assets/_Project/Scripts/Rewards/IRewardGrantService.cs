using System.Collections.Generic;

namespace LL.Rewards
{
    internal interface IRewardGrantService
    {
        bool TryGrant(RewardBundleId id, out IReadOnlyList<IReward> rewards);
    }
}