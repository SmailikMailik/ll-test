using LL.Game.Configuration;
using LL.Game.ExperienceCards;
using LL.Game.Progression;
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
        [SerializeField] private RankProgressionConfig _rankProgressionConfig;
        [SerializeField] private ExperienceCardCatalogConfig _experienceCardCatalogConfig;

        [Header("User")]
        [FormerlySerializedAs("_userDataConfig")]
        [SerializeField]
        private UserDefaultsConfig _userDefaultsConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_windowCatalog);
            builder.Register<WindowProvider>(Lifetime.Scoped);
            builder.Register<WindowNavigator>(Lifetime.Scoped);

            builder.RegisterInstance<IDataLoader<RankProgressionData>>(_rankProgressionConfig);
            builder.RegisterInstance<IDataLoader<ExperienceCardCatalogData>>(_experienceCardCatalogConfig);
            builder.RegisterInstance<IDefaultDataLoader<UserInitialData>>(_userDefaultsConfig);

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