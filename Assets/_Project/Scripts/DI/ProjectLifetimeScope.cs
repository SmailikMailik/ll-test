using LL.Purchases;
using LL.Rewards;
using LL.User;
using LL.UI.Windows;
using LL.UI.Windows.Configuration;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

namespace LL.DI
{
    internal sealed class ProjectLifetimeScope : LifetimeScope
    {
        [FormerlySerializedAs("_windowsSettings")]
        [SerializeField] private WindowCatalog _windowCatalog;

        [Header("User")]
        [SerializeField] private UserDataConfig _userDataConfig;
        [SerializeField] private LevelProgressionConfig _levelProgressionConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_windowCatalog);
            builder.Register<WindowProvider>(Lifetime.Scoped);
            builder.Register<WindowNavigator>(Lifetime.Scoped);

            builder.RegisterInstance<IUserDataSource>(_userDataConfig);
            builder.RegisterInstance<ILevelProgression>(_levelProgressionConfig);

            builder.Register(
                resolver => resolver.Resolve<IUserDataSource>().Load(),
                Lifetime.Singleton);

            builder.Register<UserWallet>(Lifetime.Singleton).As<IUserWallet>();
            builder.Register<UserProgress>(Lifetime.Singleton).As<IUserProgress>();
            builder.Register<RewardService>(Lifetime.Singleton).As<IRewardService>();
            builder.Register<PurchaseService>(Lifetime.Singleton).As<IPurchaseService>();
        }
    }
}