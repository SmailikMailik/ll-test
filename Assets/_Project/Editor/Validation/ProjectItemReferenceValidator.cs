using System;
using System.Collections.Generic;
using LL.Game.Cards.Configuration;
using LL.Game.Data.Validation;
using LL.Game.Items;
using LL.Game.Rewards.Configuration;
using LL.Presentation.Icons.Configuration;
using LL.User.Configuration;
using UnityEditor;

namespace LLEditor.Validation
{
    internal sealed class ProjectItemReferenceValidator
    {
        private const string BuiltInUserItemCode = "user-defaults.item.built-in.exists";
        private const string CardUserItemCode = "card.user-item.exists";
        private const string CardIconCode = "card.icon.exists";
        private const string RewardUserItemCode = "reward.user-item.exists";
        private const string RewardIconCode = "reward.icon.exists";

        internal void Validate(
            CardCatalogConfig cards,
            RewardBundleCatalogConfig bundles,
            ItemIconCatalogConfig icons,
            UserDefaultsConfig userDefaults,
            ValidationContext context)
        {
            if (cards == null)
                throw new ArgumentNullException(nameof(cards));

            if (bundles == null)
                throw new ArgumentNullException(nameof(bundles));

            if (icons == null)
                throw new ArgumentNullException(nameof(icons));

            if (userDefaults == null)
                throw new ArgumentNullException(nameof(userDefaults));

            var userItemIds = CollectUserItemIds(userDefaults);
            var iconIds = CollectIconIds(icons);

            ValidateBuiltInItems(userDefaults, userItemIds, context);
            ValidateCards(cards, userItemIds, iconIds, context);
            ValidateRewards(bundles, userItemIds, iconIds, context);
        }

        private static HashSet<ItemId> CollectUserItemIds(UserDefaultsConfig config)
        {
            var ids = new HashSet<ItemId>();

            foreach (var entry in config.Items ?? Array.Empty<ItemAmountEntry>())
            {
                if (entry != null)
                    ids.Add(entry.Id);
            }

            return ids;
        }

        private static HashSet<ItemId> CollectIconIds(ItemIconCatalogConfig config)
        {
            var ids = new HashSet<ItemId>();

            foreach (var entry in config.Icons ?? Array.Empty<ItemIconEntry>())
            {
                if (entry != null)
                    ids.Add(entry.Id);
            }

            return ids;
        }

        private static void ValidateBuiltInItems(
            UserDefaultsConfig config,
            ISet<ItemId> userItemIds,
            ValidationContext context)
        {
            var itemsContext = context
                .At(AssetDatabase.GetAssetPath(config))
                .At("Items");

            foreach (var id in ItemIds.All)
            {
                ValidationRules.ReferenceExists(
                    id,
                    userItemIds,
                    itemsContext,
                    BuiltInUserItemCode);
            }
        }

        private static void ValidateCards(
            CardCatalogConfig config,
            ISet<ItemId> userItemIds,
            ISet<ItemId> iconIds,
            ValidationContext context)
        {
            var cards = config.Cards;

            if (cards == null)
                return;

            var configContext = context.At(AssetDatabase.GetAssetPath(config));

            for (var index = 0; index < cards.Length; index++)
            {
                var card = cards[index];

                if (card == null || card.Id.IsEmpty)
                    continue;

                var idContext = configContext.At(index).At("Id");

                ValidationRules.ReferenceExists(
                    card.Id,
                    userItemIds,
                    idContext,
                    CardUserItemCode);

                ValidationRules.ReferenceExists(
                    card.Id,
                    iconIds,
                    idContext,
                    CardIconCode);
            }
        }

        private static void ValidateRewards(
            RewardBundleCatalogConfig config,
            ISet<ItemId> userItemIds,
            ISet<ItemId> iconIds,
            ValidationContext context)
        {
            var bundles = config.Bundles;

            if (bundles == null)
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

                    ValidationRules.ReferenceExists(
                        reward.Id,
                        userItemIds,
                        idContext,
                        RewardUserItemCode);

                    ValidationRules.ReferenceExists(
                        reward.Id,
                        iconIds,
                        idContext,
                        RewardIconCode);
                }
            }
        }
    }
}