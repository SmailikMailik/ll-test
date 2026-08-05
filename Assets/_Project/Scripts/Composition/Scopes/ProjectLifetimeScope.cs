using LL.Composition.Factories;
using LL.Composition.Installers;
using LL.Game.Data.Configuration;
using LL.Infrastructure.Saving.Serialization;
using LL.Infrastructure.Saving.Storage;
using LL.Presentation.Flags.Configuration;
using LL.Presentation.Flags.Loading;
using LL.Presentation.Heroes.Configuration;
using LL.Presentation.Heroes.Loading;
using LL.Presentation.Items.Configuration;
using LL.Presentation.Items.Loading;
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
        [SerializeField] private FlagCatalogConfig _flagCatalogConfig;
        [SerializeField] private HeroPortraitCatalogConfig _heroPortraitCatalogConfig;

        [Header("User")]
        [SerializeField] private UserDefaultsConfig _userDefaultsConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            var gameDataLoader = GameDataLoaderFactory.CreateFromScriptableObjects(_gameDataManifestConfig);
            var itemIconCatalogLoader = new ScriptableObjectItemIconCatalogLoader(_itemIconCatalogConfig);
            var flagCatalogLoader = new ScriptableObjectFlagCatalogLoader(_flagCatalogConfig);
            var heroPortraitCatalogLoader = new ScriptableObjectHeroPortraitCatalogLoader(_heroPortraitCatalogConfig);
            var windowCatalogLoader = new ScriptableObjectWindowCatalogLoader(_windowCatalogConfig);
            var userDefaultsSource = UserDefaultsSourceFactory.CreateFromScriptableObject(_userDefaultsConfig);
            var userSaveRepository = UserSaveRepositoryFactory.CreateSerialized(
                new JsonSaveSerializer(),
                new FileSaveStorage());

            new WindowInstaller(windowCatalogLoader).Install(builder);
            new PresentationInstaller(
                itemIconCatalogLoader,
                flagCatalogLoader,
                heroPortraitCatalogLoader).Install(builder);
            new ValidationReportingInstaller().Install(builder);
            new GameDataInstaller(gameDataLoader).Install(builder);
            new UserInstaller(userDefaultsSource, userSaveRepository).Install(builder);
            new GameServicesInstaller().Install(builder);
        }
    }
}