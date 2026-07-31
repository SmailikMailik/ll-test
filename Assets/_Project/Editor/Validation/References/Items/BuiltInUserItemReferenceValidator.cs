using LL.Game.Items;
using LL.User.Configuration;
using LL.Validation;
using LLEditor.Validation.Sources;
using UnityEditor;

namespace LLEditor.Validation.References.Items
{
    internal sealed class BuiltInUserItemReferenceValidator : IProjectDataReferenceValidation
    {
        private const string UserItemCode = "user-defaults.item.built-in.exists";

        public void Validate(
            ProjectDataSources sources,
            ValidationContext context)
        {
            var userDefaults = sources.GetSingle<UserDefaultsConfig>();

            if (ItemReferenceIdCollector.TryCollectValidIds(
                    userDefaults?.Items,
                    item => item.Id,
                    out var userItemIds) is false)
            {
                return;
            }

            var itemsContext = context
                .At(AssetDatabase.GetAssetPath(userDefaults))
                .At(nameof(UserDefaultsConfig.Items));

            foreach (var id in ItemIds.All)
            {
                ValidationRules.ReferenceExists(
                    id,
                    userItemIds,
                    itemsContext,
                    UserItemCode);
            }
        }
    }
}