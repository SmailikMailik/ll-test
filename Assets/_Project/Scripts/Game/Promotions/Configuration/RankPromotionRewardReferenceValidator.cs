using System.Collections.Generic;
using LL.Game.Rewards.Configuration;
using LL.Game.Rewards;
using LL.Validation;

namespace LL.Game.Promotions.Configuration
{
    internal sealed class RankPromotionRewardReferenceValidator
    {
        private const string RewardBundleExistsCode = "rank-promotion.reward-bundle.exists";

        internal void Validate(
            IReadOnlyList<RankPromotionEntry> promotions,
            IEnumerable<RewardBundleEntry> bundles,
            ValidationContext context)
        {
            if (promotions == null || bundles == null)
                return;

            var bundleIds = new HashSet<RewardBundleId>();

            foreach (var bundle in bundles)
            {
                if (bundle != null)
                    bundleIds.Add(bundle.Id);
            }

            for (var index = 0; index < promotions.Count; index++)
            {
                var promotion = promotions[index];

                if (promotion == null || promotion.RewardBundleId.IsEmpty)
                    continue;

                ValidationRules.ReferenceExists(
                    promotion.RewardBundleId,
                    bundleIds,
                    context.At(index).At("RewardBundleId"),
                    RewardBundleExistsCode);
            }
        }
    }
}