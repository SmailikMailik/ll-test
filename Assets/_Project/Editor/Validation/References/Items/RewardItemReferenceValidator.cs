using LL.Game.Rewards.Configuration;
using LL.Game.Identifiers;
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
            var config = sources.GetSingle<RewardCatalogConfig>();
            var rewards = config?.Rewards;

            if (rewards == null)
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

            for (var rewardIndex = 0; rewardIndex < rewards.Length; rewardIndex++)
            {
                var items = rewards[rewardIndex]?.Items;

                if (items == null)
                    continue;

                for (var itemIndex = 0; itemIndex < items.Length; itemIndex++)
                {
                    var item = items[itemIndex];

                    if (item == null || IdentifierValidator.IsValid(item.Id) is false)
                        continue;

                    var idContext = configContext
                        .At(rewardIndex)
                        .At(nameof(RewardEntry.Items))
                        .At(itemIndex)
                        .At(nameof(RewardItemEntry.Id));

                    if (hasUserItems)
                    {
                        ValidationRules.ReferenceExists(
                            item.Id,
                            userItemIds,
                            idContext,
                            UserItemCode);
                    }

                    if (hasIcons)
                    {
                        ValidationRules.ReferenceExists(
                            item.Id,
                            iconIds,
                            idContext,
                            IconCode);
                    }
                }
            }
        }
    }
}