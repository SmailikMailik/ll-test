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
        private const string RewardExistsCode = "rank-promotion.reward.exists";

        public void Validate(
            ProjectDataSources sources,
            ValidationContext context)
        {
            var promotions = sources.GetSingle<RankPromotionCatalogConfig>();
            var rewards = sources.GetSingle<RewardCatalogConfig>();

            if (promotions == null || rewards == null)
                return;

            ValidateReferences(
                promotions.Promotions,
                rewards.Rewards,
                context.At(AssetDatabase.GetAssetPath(promotions)));
        }

        private static void ValidateReferences(
            IReadOnlyList<RankPromotionEntry> promotions,
            IEnumerable<RewardEntry> rewards,
            ValidationContext context)
        {
            if (promotions == null || rewards == null)
                return;

            var rewardIds = new HashSet<RewardId>();

            foreach (var reward in rewards)
            {
                if (reward != null && IdentifierValidator.IsValid(reward.Id))
                    rewardIds.Add(reward.Id);
            }

            for (var index = 0; index < promotions.Count; index++)
            {
                var promotion = promotions[index];

                if (promotion == null ||
                    IdentifierValidator.IsValid(promotion.RewardId) is false)
                {
                    continue;
                }

                ValidationRules.ReferenceExists(
                    promotion.RewardId,
                    rewardIds,
                    context.At(index).At(nameof(RankPromotionEntry.RewardId)),
                    RewardExistsCode);
            }
        }
    }
}