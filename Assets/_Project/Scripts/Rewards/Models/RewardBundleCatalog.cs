using System;
using System.Collections.Generic;
using System.Linq;
using LL.Identifiers;

namespace LL.Rewards.Models
{
    internal sealed class RewardBundleCatalog
    {
        private readonly IReadOnlyDictionary<RewardBundleId, RewardBundle> _bundles;

        internal RewardBundleCatalog(IEnumerable<RewardBundle> bundles)
        {
            var copy = bundles?.ToArray() ?? Array.Empty<RewardBundle>();

            IdentifierCatalogValidator.EnsureValidIds(
                copy,
                bundle => bundle.Id,
                nameof(RewardBundleCatalog),
                nameof(bundles));

            _bundles = copy.ToDictionary(bundle => bundle.Id);
        }

        internal bool TryGetBundle(RewardBundleId id, out RewardBundle bundle)
        {
            return _bundles.TryGetValue(id, out bundle);
        }
    }
}