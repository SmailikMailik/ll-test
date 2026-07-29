using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Rewards;

namespace LL.User.Snapshots.Rewards
{
    internal sealed class UserRewardClaimsSnapshot
    {
        internal IReadOnlyList<RewardBundleId> ClaimedIds { get; }

        internal UserRewardClaimsSnapshot(IEnumerable<RewardBundleId> claimedIds)
        {
            var copy = claimedIds?
                .Where(id => id.IsEmpty is false)
                .Distinct()
                .ToArray()
                ?? Array.Empty<RewardBundleId>();

            ClaimedIds = Array.AsReadOnly(copy);
        }
    }
}