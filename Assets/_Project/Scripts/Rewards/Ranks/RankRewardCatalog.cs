using System;
using System.Collections.Generic;
using System.Linq;

namespace LL.Rewards.Ranks
{
    internal sealed class RankRewardCatalog
    {
        private readonly IReadOnlyDictionary<int, RewardBundleId> _bundleIds;

        internal RankRewardCatalog(IEnumerable<KeyValuePair<int, RewardBundleId>> bundleIds)
        {
            var copy = bundleIds?.ToArray() ?? Array.Empty<KeyValuePair<int, RewardBundleId>>();

            if (copy.Any(entry => entry.Key < 1 || entry.Value.IsEmpty))
                throw new ArgumentException("Rank reward entries must have a positive rank and a bundle ID.");

            if (copy.Select(entry => entry.Key).Distinct().Count() != copy.Length)
                throw new ArgumentException("Rank reward entries must have unique ranks.");

            _bundleIds = copy.ToDictionary(entry => entry.Key, entry => entry.Value);
        }

        internal bool TryGetBundleId(int rank, out RewardBundleId id)
        {
            return _bundleIds.TryGetValue(rank, out id);
        }
    }
}