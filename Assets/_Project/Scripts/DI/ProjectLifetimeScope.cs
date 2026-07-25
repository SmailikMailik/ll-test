using LL.Purchases;
using LL.Rewards;
using LL.UI.Windows;
using LL.UI.Windows.Configuration;
using LL.User.Configuration;
using LL.User.Core.Data;
using LL.User.Core.Progress;
using LL.User.Core.Wallet;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace LL.DI
{
    internal sealed class ProjectLifetimeScope : LifetimeScope
    {
        [SerializeField] private WindowCatalog _windowCatalog;

        [Header("User")]
        [SerializeField] private UserDataConfig _userDataConfig;
        [SerializeField] private RankProgressionConfig _rankProgressionConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_windowCatalog);
            builder.Register<WindowProvider>(Lifetime.Scoped);
            builder.Register<WindowNavigator>(Lifetime.Scoped);

            builder.RegisterInstance<IUserDataSource>(_userDataConfig);
            builder.RegisterInstance<IRankProgressionSource>(_rankProgressionConfig);

            builder.Register(resolver => resolver.Resolve<IUserDataSource>().Load(), Lifetime.Singleton);

            builder.Register<UserWallet>(Lifetime.Singleton).As<IUserWallet>();
            builder.Register<RankProgression>(Lifetime.Singleton).As<IRankProgression>();
            builder.Register<UserProgress>(Lifetime.Singleton).As<IUserProgress>();
            builder.Register<RewardService>(Lifetime.Singleton).As<IRewardService>();
            builder.Register<PurchaseService>(Lifetime.Singleton).As<IPurchaseService>();
        }
    }
}