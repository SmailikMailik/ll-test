using LL.Game.Cards.Configuration;
using LL.Game.Identifiers;
using LL.Presentation.Icons.Configuration;
using LL.User.Configuration;
using LL.Validation;
using LLEditor.Validation.Sources;
using UnityEditor;

namespace LLEditor.Validation.References.Items
{
    internal sealed class CardItemReferenceValidator : IProjectDataReferenceValidation
    {
        private const string UserItemCode = "card.user-item.exists";
        private const string IconCode = "card.icon.exists";

        public void Validate(
            ProjectDataSources sources,
            ValidationContext context)
        {
            var config = sources.GetSingle<CardCatalogConfig>();
            var cards = config?.Cards;

            if (cards == null)
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

            for (var index = 0; index < cards.Count; index++)
            {
                var card = cards[index];

                if (card == null || IdentifierValidator.IsValid(card.Id) is false)
                    continue;

                var idContext = configContext.At(index).At(nameof(CardEntry.Id));

                if (hasUserItems)
                {
                    ValidationRules.ReferenceExists(
                        card.Id,
                        userItemIds,
                        idContext,
                        UserItemCode);
                }

                if (hasIcons)
                {
                    ValidationRules.ReferenceExists(
                        card.Id,
                        iconIds,
                        idContext,
                        IconCode);
                }
            }
        }
    }
}