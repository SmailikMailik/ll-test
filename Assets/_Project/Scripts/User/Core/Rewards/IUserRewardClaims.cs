using System.Collections.Generic;
using LL.Rewards.Models;
using R3;

namespace LL.User.Core.Rewards
{
    internal interface IUserRewardClaims
    {
        IReadOnlyCollection<RewardBundleId> ClaimedIds { get; }
        Observable<RewardBundleId> RewardClaimed { get; }

        bool Contains(RewardBundleId id);
        bool TryClaim(RewardBundleId id);
    }
}