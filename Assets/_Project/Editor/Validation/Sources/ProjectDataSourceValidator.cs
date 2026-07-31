using System;
using LL.Game.Cards.Configuration;
using LL.Game.Data.Configuration;
using LL.Game.Heroes.Configuration;
using LL.Game.RankUp.Configuration;
using LL.Game.Quests.Configuration;
using LL.Game.Ranks.Configuration;
using LL.Game.Rewards.Configuration;
using LL.Presentation.Items.Configuration;
using LL.Presentation.Countries.Configuration;
using LL.Presentation.Heroes.Configuration;
using LL.UI.Windows.Configuration;
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
            typeof(GameDataManifestConfig),
            typeof(CardCatalogConfig),
            typeof(HeroCatalogConfig),
            typeof(QuestCatalogConfig),
            typeof(RankCatalogConfig),
            typeof(RankUpCatalogConfig),
            typeof(RewardCatalogConfig),
            typeof(ItemIconCatalogConfig),
            typeof(CountryFlagCatalogConfig),
            typeof(HeroPortraitCatalogConfig),
            typeof(WindowCatalogConfig),
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