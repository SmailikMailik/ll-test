using LL.Game.Rewards.Configuration;
using LL.Presentation.Icons.Configuration;
using LL.User.Configuration;
using LL.Validation;
using LLEditor.Validation.Sources;
using UnityEditor;

namespace LLEditor.Validation.References.Items
{
    internal sealed class RewardItemReferenceValidator : IProjectDataReferenceValidation
    {
        private const string UserItemCode = "reward.user-item.exists";
        private const string IconCode = "reward.icon.exists";

        public void Validate(
            ProjectDataSources sources,
            ValidationContext context)
        {
            var config = sources.GetSingle<RewardBundleCatalogConfig>();
            var bundles = config?.Bundles;

            if (bundles == null)
                return;

            var userDefaults = sources.GetSingle<UserDefaultsConfig>();
            var hasUserItems = ItemReferenceIdCollector.TryCollect(
                userDefaults?.Items,
                item => item.Id,
                out var userItemIds);

            var icons = sources.GetSingle<ItemIconCatalogConfig>();
            var hasIcons = ItemReferenceIdCollector.TryCollect(
                icons?.Icons,
                icon => icon.Id,
                out var iconIds);

            if (hasUserItems is false && hasIcons is false)
                return;

            var configContext = context.At(AssetDatabase.GetAssetPath(config));

            for (var bundleIndex = 0; bundleIndex < bundles.Length; bundleIndex++)
            {
                var rewards = bundles[bundleIndex]?.Rewards;

                if (rewards == null)
                    continue;

                for (var rewardIndex = 0; rewardIndex < rewards.Length; rewardIndex++)
                {
                    var reward = rewards[rewardIndex];

                    if (reward == null || reward.Id.IsEmpty)
                        continue;

                    var idContext = configContext
                        .At(bundleIndex)
                        .At("Rewards")
                        .At(rewardIndex)
                        .At("Id");

                    if (hasUserItems)
                    {
                        ValidationRules.ReferenceExists(
                            reward.Id,
                            userItemIds,
                            idContext,
                            UserItemCode);
                    }

                    if (hasIcons)
                    {
                        ValidationRules.ReferenceExists(
                            reward.Id,
                            iconIds,
                            idContext,
                            IconCode);
                    }
                }
            }
        }
    }
}