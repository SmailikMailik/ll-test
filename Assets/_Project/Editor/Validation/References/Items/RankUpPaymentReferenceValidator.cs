using System.Collections.Generic;
using LL.Game.Identifiers;
using LL.Game.Items;
using LL.Game.Payments.Configuration;
using LL.Game.RankUp.Configuration;
using LL.Presentation.Items.Configuration;
using LL.User.Configuration;
using LL.Validation;
using LLEditor.Validation.Sources;
using UnityEditor;

namespace LLEditor.Validation.References.Items
{
    internal sealed class RankUpPaymentReferenceValidator : IProjectDataReferenceValidation
    {
        private const string UserItemCode = "rank-up.payment.user-item.exists";
        private const string IconCode = "rank-up.payment.icon.exists";

        public void Validate(
            ProjectDataSources sources,
            ValidationContext context)
        {
            var config = sources.GetSingle<RankUpCatalogConfig>();
            var rankUps = config?.RankUps;

            if (rankUps is null)
                return;

            var userDefaults = sources.GetSingle<UserDefaultsConfig>();
            var hasUserItems = ItemReferenceIdCollector.TryCollectValidIds(
                userDefaults?.Items,
                item => item.Id,
                out var userItemIds);

            var icons = sources.GetSingle<ItemIconCatalogConfig>();
            var hasIcons = ItemReferenceIdCollector.TryCollectValidIds(
                icons?.Icons,
                icon => icon.Id,
                out var iconIds);

            if (hasUserItems is false && hasIcons is false)
                return;

            var configContext = context.At(AssetDatabase.GetAssetPath(config));

            for (var index = 0; index < rankUps.Count; index++)
            {
                var rankUp = rankUps[index];

                if (rankUp?.Options is null)
                    continue;

                for (var optionIndex = 0; optionIndex < rankUp.Options.Count; optionIndex++)
                {
                    var option = rankUp.Options[optionIndex];

                    if (option?.Payments is null)
                        continue;

                    for (var paymentIndex = 0; paymentIndex < option.Payments.Count; paymentIndex++)
                    {
                        var payment = option.Payments[paymentIndex];

                        if (payment is null)
                            continue;

                        ValidatePayment(
                            payment.Payment,
                            configContext
                                .At(index)
                                .At(nameof(RankUpEntry.Options))
                                .At(optionIndex)
                                .At(nameof(RankUpOptionEntry.Payments))
                                .At(paymentIndex)
                                .At(nameof(PaymentRankUpRequirementEntry.Payment)),
                            hasUserItems,
                            userItemIds,
                            hasIcons,
                            iconIds);
                    }
                }
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
            if (payment is null || IdentifierValidator.IsValid(payment.ItemId) is false)
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