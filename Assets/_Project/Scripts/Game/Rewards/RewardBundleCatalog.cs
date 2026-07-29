using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Identifiers;

namespace LL.Game.Rewards
{
    internal sealed class RewardBundleCatalog
    {
        private readonly IReadOnlyDictionary<RewardBundleId, RewardBundle> _bundles;

        internal RewardBundleCatalog(IEnumerable<RewardBundle> bundles)
        {
            var copy = bundles?.ToArray() ?? Array.Empty<RewardBundle>();

            IdentifierCollectionValidator.Validate(
                copy,
                bundle => bundle.Id,
                nameof(bundles));

            _bundles = copy.ToDictionary(bundle => bundle.Id);
        }

        internal bool TryGetBundle(RewardBundleId id, out RewardBundle bundle)
        {
            return _bundles.TryGetValue(id, out bundle);
        }
    }
}