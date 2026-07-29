using System;
using LL.Game.Cards.Configuration;
using LL.Game.Promotions.Configuration;
using LL.Game.Ranks.Configuration;
using LL.Game.Rewards.Configuration;
using LL.Presentation.Icons.Configuration;
using LL.User.Configuration;
using LL.Validation;

namespace LLEditor.Validation.Sources
{
    internal sealed class ProjectDataSourceValidator
    {
        private const string RequiredCode = "project-data.source.required";
        private const string UniqueCode = "project-data.source.unique";

        private static readonly Type[] _requiredSourceTypes =
        {
            typeof(CardCatalogConfig),
            typeof(RankCatalogConfig),
            typeof(RankPromotionCatalogConfig),
            typeof(RewardCatalogConfig),
            typeof(ItemIconCatalogConfig),
            typeof(UserDefaultsConfig)
        };

        internal void Validate(
            ProjectDataSources sources,
            ValidationContext context)
        {
            foreach (var sourceType in _requiredSourceTypes)
            {
                var count = sources.Count(sourceType);
                var sourceContext = context.At(sourceType.Name);

                if (count == 0)
                {
                    sourceContext.Report(
                        ValidationSeverity.Error,
                        RequiredCode,
                        $"Project data source '{sourceType.Name}' is required.");
                    continue;
                }

                if (count > 1)
                {
                    sourceContext.Report(
                        ValidationSeverity.Error,
                        UniqueCode,
                        $"Project data source '{sourceType.Name}' must be unique, but {count} were found.");
                }
            }
        }
    }
}