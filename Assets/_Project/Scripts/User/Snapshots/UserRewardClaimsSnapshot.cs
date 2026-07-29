using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Identifiers;
using LL.Game.Rewards;

namespace LL.User.Snapshots
{
    internal sealed class UserRewardClaimsSnapshot
    {
        internal static UserRewardClaimsSnapshot Empty { get; } = new(Array.Empty<RewardBundleId>());

        internal IReadOnlyList<RewardBundleId> ClaimedIds { get; }

        internal UserRewardClaimsSnapshot(IEnumerable<RewardBundleId> claimedIds)
        {
            var copy = claimedIds?.ToArray() ?? Array.Empty<RewardBundleId>();

            IdentifierCollectionValidator.EnsureValid(
                copy,
                id => id,
                nameof(claimedIds));

            ClaimedIds = Array.AsReadOnly(copy);
        }
    }
}