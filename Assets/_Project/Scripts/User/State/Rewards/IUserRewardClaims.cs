using System.Collections.Generic;
using LL.Game.Rewards;
using R3;

namespace LL.User.State.Rewards
{
    internal interface IUserRewardClaims
    {
        IReadOnlyCollection<RewardBundleId> ClaimedIds { get; }
        Observable<RewardBundleId> RewardClaimed { get; }

        bool Contains(RewardBundleId id);
        bool TryClaim(RewardBundleId id);
    }
}