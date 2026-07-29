using System.Collections.Generic;
using System.Linq;
using LL.Game.Cards.Configuration;
using LL.Game.Promotions.Configuration;
using LL.Game.Ranks.Configuration;
using LL.Game.Rewards.Configuration;
using LL.Presentation.Icons.Configuration;
using LL.User.Configuration;
using LL.Validation;
using UnityEditor;
using UnityEngine;

namespace LLEditor.Validation
{
    internal sealed class ProjectDataReferenceValidator
    {
        private readonly ProjectItemReferenceValidator _itemValidator = new();
        private readonly RankPromotionRankReferenceValidator _rankPromotionRankValidator = new();
        private readonly RankPromotionRewardReferenceValidator _rankPromotionRewardValidator = new();
        private readonly UserProgressRankReferenceValidator _userProgressRankValidator = new();

        internal void Validate(
            IReadOnlyCollection<ScriptableObject> assets,
            ValidationContext context)
        {
            var cards = GetSingle<CardCatalogConfig>(assets);
            var icons = GetSingle<ItemIconCatalogConfig>(assets);
            var promotions = GetSingle<RankPromotionCatalogConfig>(assets);
            var ranks = GetSingle<RankCatalogConfig>(assets);
            var rewards = GetSingle<RewardBundleCatalogConfig>(assets);
            var userDefaults = GetSingle<UserDefaultsConfig>(assets);

            if (promotions != null && ranks != null)
            {
                var promotionsContext = context.At(AssetDatabase.GetAssetPath(promotions));

                _rankPromotionRankValidator.Validate(
                    promotions.Promotions,
                    ranks.RankRequirements,
                    promotionsContext);
            }

            if (promotions != null && rewards != null)
            {
                _rankPromotionRewardValidator.Validate(
                    promotions.Promotions,
                    rewards.Bundles,
                    context.At(AssetDatabase.GetAssetPath(promotions)));
            }

            if (ranks != null && userDefaults != null)
            {
                _userProgressRankValidator.Validate(
                    userDefaults.Progress,
                    ranks.RankRequirements,
                    context
                        .At(AssetDatabase.GetAssetPath(userDefaults))
                        .At("Progress"));
            }

            if (cards != null && rewards != null && icons != null && userDefaults != null)
                _itemValidator.Validate(cards, rewards, icons, userDefaults, context);
        }

        private static TSource GetSingle<TSource>(IEnumerable<ScriptableObject> assets)
            where TSource : ScriptableObject
        {
            var matches = assets
                .OfType<TSource>()
                .Take(2)
                .ToArray();

            return matches.Length == 1
                ? matches[0]
                : null;
        }
    }
}