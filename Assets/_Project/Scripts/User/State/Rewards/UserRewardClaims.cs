using System;
using System.Collections.Generic;
using LL.Game.Rewards;
using LL.User.Snapshots.Rewards;
using R3;
using VContainer;

namespace LL.User.State.Rewards
{
    internal sealed class UserRewardClaims : IUserRewardClaims, IDisposable
    {
        public IReadOnlyCollection<RewardBundleId> ClaimedIds => _claimedIds;
        public Observable<RewardBundleId> RewardClaimed => _rewardClaimed;

        private readonly HashSet<RewardBundleId> _claimedIds;
        private readonly Subject<RewardBundleId> _rewardClaimed = new();

        [Inject]
        internal UserRewardClaims(UserRewardClaimsSnapshot snapshot)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));

            _claimedIds = new HashSet<RewardBundleId>(snapshot.ClaimedIds);
        }

        public bool Contains(RewardBundleId id) => _claimedIds.Contains(id);

        public bool TryClaim(RewardBundleId id)
        {
            if (id.IsEmpty || _claimedIds.Add(id) is false)
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