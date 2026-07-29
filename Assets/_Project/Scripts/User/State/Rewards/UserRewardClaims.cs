using System;
using System.Collections.Generic;
using LL.Game.Rewards;
using LL.User.Snapshots;
using R3;
using VContainer;

namespace LL.User.State.Rewards
{
    internal sealed class UserRewardClaims : IUserRewardClaims, IDisposable
    {
        public IReadOnlyCollection<RewardId> ClaimedIds => _claimedIds;
        public Observable<RewardId> RewardClaimed => _rewardClaimed;

        private readonly HashSet<RewardId> _claimedIds;
        private readonly Subject<RewardId> _rewardClaimed = new();

        [Inject]
        internal UserRewardClaims(UserRewardClaimsSnapshot snapshot)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));

            _claimedIds = new HashSet<RewardId>(snapshot.ClaimedIds);
        }

        public bool Contains(RewardId id) => _claimedIds.Contains(id);

        public bool TryClaim(RewardId id)
        {
            if (string.IsNullOrWhiteSpace(id.Value) || _claimedIds.Add(id) is false)
                return false;

            _rewardClaimed.OnNext(id);
            return true;
        }

        public void Dispose()
        {
            _rewardClaimed.Dispose();
        }
    }
}