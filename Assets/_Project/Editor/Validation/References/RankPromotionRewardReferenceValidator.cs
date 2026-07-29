using System.Collections.Generic;
using LL.Game.Identifiers;
using LL.Game.Promotions.Configuration;
using LL.Game.Rewards;
using LL.Game.Rewards.Configuration;
using LL.Validation;
using LLEditor.Validation.Sources;
using UnityEditor;

namespace LLEditor.Validation.References
{
    internal sealed class RankPromotionRewardReferenceValidator : IProjectDataReferenceValidation
    {
        private const string RewardBundleExistsCode = "rank-promotion.reward-bundle.exists";

        public void Validate(
            ProjectDataSources sources,
            ValidationContext context)
        {
            var promotions = sources.GetSingle<RankPromotionCatalogConfig>();
            var rewards = sources.GetSingle<RewardBundleCatalogConfig>();

            if (promotions == null || rewards == null)
                return;

            ValidateReferences(
                promotions.Promotions,
                rewards.Bundles,
                context.At(AssetDatabase.GetAssetPath(promotions)));
        }

        private static void ValidateReferences(
            IReadOnlyList<RankPromotionEntry> promotions,
            IEnumerable<RewardBundleEntry> bundles,
            ValidationContext context)
        {
            if (promotions == null || bundles == null)
                return;

            var bundleIds = new HashSet<RewardBundleId>();

            foreach (var bundle in bundles)
            {
                if (bundle != null && IdentifierValidator.IsValid(bundle.Id))
                    bundleIds.Add(bundle.Id);
            }

            for (var index = 0; index < promotions.Count; index++)
            {
                var promotion = promotions[index];

                if (promotion == null ||
                    IdentifierValidator.IsValid(promotion.RewardBundleId) is false)
                {
                    continue;
                }

                ValidationRules.ReferenceExists(
                    promotion.RewardBundleId,
                    bundleIds,
                    context.At(index).At(nameof(RankPromotionEntry.RewardBundleId)),
                    RewardBundleExistsCode);
            }
        }
    }
}