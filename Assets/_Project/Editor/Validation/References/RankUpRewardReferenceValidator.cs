using System.Collections.Generic;
using LL.Game.Identifiers;
using LL.Game.RankUp.Configuration;
using LL.Game.Rewards;
using LL.Game.Rewards.Configuration;
using LL.Validation;
using LLEditor.Validation.Sources;
using UnityEditor;

namespace LLEditor.Validation.References
{
    internal sealed class RankUpRewardReferenceValidator : IProjectDataReferenceValidation
    {
        private const string RewardExistsCode = "rank-up.reward.exists";

        public void Validate(
            ProjectDataSources sources,
            ValidationContext context)
        {
            var rankUps = sources.GetSingle<RankUpCatalogConfig>();
            var rewards = sources.GetSingle<RewardCatalogConfig>();

            if (rankUps == null || rewards == null)
                return;

            ValidateReferences(
                rankUps.RankUps,
                rewards.Rewards,
                context.At(AssetDatabase.GetAssetPath(rankUps)));
        }

        private static void ValidateReferences(
            IReadOnlyList<RankUpEntry> rankUps,
            IEnumerable<RewardEntry> rewards,
            ValidationContext context)
        {
            if (rankUps == null || rewards == null)
                return;

            var rewardIds = new HashSet<RewardId>();

            foreach (var reward in rewards)
            {
                if (reward != null && IdentifierValidator.IsValid(reward.Id))
                    rewardIds.Add(reward.Id);
            }

            for (var index = 0; index < rankUps.Count; index++)
            {
                var rankUp = rankUps[index];

                if (rankUp == null ||
                    IdentifierValidator.IsValid(rankUp.RewardId) is false)
                {
                    continue;
                }

                ValidationRules.ReferenceExists(
                    rankUp.RewardId,
                    rewardIds,
                    context.At(index).At(nameof(RankUpEntry.RewardId)),
                    RewardExistsCode);
            }
        }
    }
}