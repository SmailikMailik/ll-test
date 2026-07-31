using LL.Composition.Factories;
using LL.Composition.Installers;
using LL.Game.Data.Configuration;
using LL.Presentation.Countries.Configuration;
using LL.Presentation.Countries.Loading;
using LL.Presentation.Heroes.Configuration;
using LL.Presentation.Heroes.Loading;
using LL.Presentation.Icons.Configuration;
using LL.Presentation.Icons.Loading;
using LL.UI.Windows.Configuration;
using LL.UI.Windows.Loading;
using LL.User.Configuration;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using VContainer;
using VContainer.Unity;

namespace LL.Composition.Scopes
{
    [MovedFrom(true, "LL.Composition")]
    [DisallowMultipleComponent]
    internal sealed class ProjectLifetimeScope : LifetimeScope
    {
        [Header("Windows")]
        [SerializeField] private WindowCatalogConfig _windowCatalogConfig;

        [Header("Game Data")]
        [SerializeField] private GameDataManifestConfig _gameDataManifestConfig;

        [Header("Presentation")]
        [SerializeField] private ItemIconCatalogConfig _itemIconCatalogConfig;
        [SerializeField] private CountryFlagCatalogConfig _countryFlagCatalogConfig;
        [SerializeField] private HeroPortraitCatalogConfig _heroPortraitCatalogConfig;

        [Header("User")]
        [SerializeField] private UserDefaultsConfig _userDefaultsConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            var gameDataLoader = GameDataLoaderFactory.CreateFromScriptableObjects(_gameDataManifestConfig);
            var itemIconCatalogLoader = new ScriptableObjectItemIconCatalogLoader(_itemIconCatalogConfig);
            var countryFlagCatalogLoader = new ScriptableObjectCountryFlagCatalogLoader(_countryFlagCatalogConfig);
            var heroPortraitCatalogLoader = new ScriptableObjectHeroPortraitCatalogLoader(_heroPortraitCatalogConfig);
            var windowCatalogLoader = new ScriptableObjectWindowCatalogLoader(_windowCatalogConfig);
            var userDefaultsSource = UserDefaultsSourceFactory.CreateFromScriptableObject(_userDefaultsConfig);
            var userSaveRepository = UserSaveRepositoryFactory.CreateJsonFile();

            new WindowInstaller(windowCatalogLoader).Install(builder);
            new PresentationInstaller(
                itemIconCatalogLoader,
                countryFlagCatalogLoader,
                heroPortraitCatalogLoader).Install(builder);
            new ValidationReportingInstaller().Install(builder);
            new GameDataInstaller(gameDataLoader).Install(builder);
            new UserInstaller(userDefaultsSource, userSaveRepository).Install(builder);
            new GameServicesInstaller().Install(builder);
        }
    }
}