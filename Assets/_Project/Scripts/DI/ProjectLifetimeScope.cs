using LL.Game.Configuration;
using LL.Game.Currencies;
using LL.Game.ExperienceCards;
using LL.Game.Ranks;
using LL.Presentation.Configuration;
using LL.Presentation.Icons;
using LL.Purchases;
using LL.Rewards;
using LL.Saving;
using LL.UI.Windows;
using LL.UI.Windows.Configuration;
using LL.User.Configuration;
using LL.User.Core;
using LL.User.Core.ExperienceCards;
using LL.User.Core.Identity;
using LL.User.Core.Progress;
using LL.User.Core.Wallet;
using LL.User.Persistence;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

namespace LL.DI
{
    internal sealed class ProjectLifetimeScope : LifetimeScope
    {
        [SerializeField] private WindowCatalog _windowCatalog;

        [Header("Game Data")]
        [FormerlySerializedAs("_rankProgressionConfig")]
        [SerializeField] private RankCatalogConfig _rankCatalogConfig;
        [SerializeField] private ExperienceCardCatalogConfig _experienceCardCatalogConfig;

        [Header("Presentation")]
        [SerializeField] private CurrencyIconCatalogConfig _currencyIconCatalogConfig;
        [SerializeField] private ExperienceCardIconCatalogConfig _experienceCardIconCatalogConfig;

        [Header("User")]
        [FormerlySerializedAs("_userDataConfig")]
        [SerializeField]
        private UserDefaultsConfig _userDefaultsConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_windowCatalog);
            builder.Register<WindowProvider>(Lifetime.Scoped);
            builder.Register<WindowNavigator>(Lifetime.Scoped);

            builder.RegisterInstance<IDataLoader<RankCatalog>>(_rankCatalogConfig);
            builder.RegisterInstance<IDataLoader<ExperienceCardCatalog>>(_experienceCardCatalogConfig);
            builder.RegisterInstance<IDataLoader<IconCatalog<CurrencyId>>>(_currencyIconCatalogConfig);
            builder.RegisterInstance<IDataLoader<IconCatalog<ExperienceCardId>>>(
                _experienceCardIconCatalogConfig);
            builder.RegisterInstance<IDefaultDataLoader<UserInitialData>>(_userDefaultsConfig);

            builder
                .Register(
                    resolver => resolver
                        .Resolve<IDataLoader<IconCatalog<CurrencyId>>>()
                        .Load(),
                    Lifetime.Singleton)
                .As<IIconProvider<CurrencyId>>();
            builder
                .Register(
                    resolver => resolver
                        .Resolve<IDataLoader<IconCatalog<ExperienceCardId>>>()
                        .Load(),
                    Lifetime.Singleton)
                .As<IIconProvider<ExperienceCardId>>();

            builder.Register<JsonFileSaveService>(Lifetime.Singleton).As<ISaveService>();
            builder.Register<UserInitialDataLoader>(Lifetime.Singleton).As<IDataLoader<UserInitialData>>();

            builder.Register(resolver => resolver.Resolve<IDataLoader<UserInitialData>>().Load(), Lifetime.Singleton);
            builder.Register(resolver => resolver.Resolve<UserInitialData>().Identity, Lifetime.Singleton);
            builder.Register(resolver => resolver.Resolve<UserInitialData>().Wallet, Lifetime.Singleton);
            builder.Register(resolver => resolver.Resolve<UserInitialData>().Progress, Lifetime.Singleton);
            builder.Register(resolver => resolver.Resolve<UserInitialData>().ExperienceCards, Lifetime.Singleton);

            builder.Register<UserWallet>(Lifetime.Singleton).As<IUserWallet>();
            builder.Register<RankProgression>(Lifetime.Singleton).As<IRankProgression>();
            builder.Register<UserProgress>(Lifetime.Singleton).As<IUserProgress>();
            builder.Register<RewardService>(Lifetime.Singleton).As<IRewardService>();
            builder.Register<PurchaseService>(Lifetime.Singleton).As<IPurchaseService>();

            builder.RegisterEntryPoint<UserSaveController>();
        }
    }
}