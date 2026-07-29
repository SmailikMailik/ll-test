using System.Collections.Generic;
using LL.Game.Rewards;
using R3;

namespace LL.User.State.Rewards
{
    internal interface IUserRewardClaims
    {
        IReadOnlyCollection<RewardId> ClaimedIds { get; }
        Observable<RewardId> RewardClaimed { get; }

        bool Contains(RewardId id);
        bool TryClaim(RewardId id);
    }
}