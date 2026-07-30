using System.Collections.Generic;
using LL.Game.Identifiers;
using LL.Game.Items;
using LL.Game.Payments.Configuration;
using LL.Game.Promotions.Configuration;
using LL.Presentation.Icons.Configuration;
using LL.User.Configuration;
using LL.Validation;
using LLEditor.Validation.Sources;
using UnityEditor;

namespace LLEditor.Validation.References.Items
{
    internal sealed class RankPromotionPaymentReferenceValidator : IProjectDataReferenceValidation
    {
        private const string UserItemCode = "rank-promotion.payment.user-item.exists";
        private const string IconCode = "rank-promotion.payment.icon.exists";

        public void Validate(
            ProjectDataSources sources,
            ValidationContext context)
        {
            var config = sources.GetSingle<RankPromotionCatalogConfig>();
            var promotions = config?.Promotions;

            if (promotions == null)
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

            for (var index = 0; index < promotions.Count; index++)
            {
                var promotion = promotions[index];

                if (promotion == null)
                    continue;

                ValidatePayment(
                    promotion.OrderPayment,
                    configContext.At(index).At(nameof(RankPromotionEntry.OrderPayment)),
                    hasUserItems,
                    userItemIds,
                    hasIcons,
                    iconIds);
                ValidatePayment(
                    promotion.InstantPayment,
                    configContext.At(index).At(nameof(RankPromotionEntry.InstantPayment)),
                    hasUserItems,
                    userItemIds,
                    hasIcons,
                    iconIds);
            }
        }

        private static void ValidatePayment(
            PaymentEntry payment,
            ValidationContext context,
            bool hasUserItems,
            ISet<ItemId> userItemIds,
            bool hasIcons,
            ISet<ItemId> iconIds)
        {
            if (payment == null || IdentifierValidator.IsValid(payment.ItemId) is false)
                return;

            var idContext = context.At(nameof(PaymentEntry.ItemId));

            if (hasUserItems)
            {
                ValidationRules.ReferenceExists(
                    payment.ItemId,
                    userItemIds,
                    idContext,
                    UserItemCode);
            }

            if (hasIcons)
            {
                ValidationRules.ReferenceExists(
                    payment.ItemId,
                    iconIds,
                    idContext,
                    IconCode);
            }
        }
    }
}