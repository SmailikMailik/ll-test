using System;
using System.Collections.Generic;
using System.Linq;
using LL.Rewards;

namespace LL.User.Core.Rewards
{
    internal sealed class RewardClaimsInitialData
    {
        internal IReadOnlyList<RewardBundleId> ClaimedIds { get; }

        internal RewardClaimsInitialData(IEnumerable<RewardBundleId> claimedIds)
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