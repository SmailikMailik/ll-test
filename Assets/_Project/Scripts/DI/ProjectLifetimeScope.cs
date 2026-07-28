using LL.Game.Cards;
using LL.Game.Configuration;
using LL.Game.Promotions;
using LL.Game.Ranks;
using LL.Game.Items;
using LL.Loading;
using LL.Presentation.Configuration;
using LL.Presentation.Icons;
using LL.Presentation.Localization;
using LL.Rewards;
using LL.Rewards.Configuration;
using LL.Saving;
using LL.Upgrades;
using LL.UI.Windows;
using LL.UI.Windows.Configuration;
using LL.User.Configuration;
using LL.User.Core;
using LL.User.Core.Cards;
using LL.User.Core.Progress;
using LL.User.Core.Items;
using LL.User.Core.Promotions;
using LL.User.Core.Rewards;
using LL.User.Persistence;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace LL.DI
{
    [DisallowMultipleComponent]
    internal sealed class ProjectLifetimeScope : LifetimeScope
    {
        [SerializeField] private WindowCatalog _windowCatalog;

        [Header("Game Data")]
        [SerializeField] private RankCatalogConfig _rankCatalogConfig;
        [SerializeField] private CardCatalogConfig _cardCatalogConfig;
        [SerializeField] private RankPromotionCatalogConfig _rankPromotionCatalogConfig;

        [Header("Rewards")]
        [SerializeField] private RewardBundleCatalogConfig _rewardBundleCatalogConfig;

        [Header("Presentation")]
        [SerializeField] private ItemIconCatalogConfig _itemIconCatalogConfig;
        [SerializeField] private CardIconCatalogConfig _cardIconCatalogConfig;

        [Header("User")]
        [SerializeField] private UserDefaultsConfig _userDefaultsConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_windowCatalog);
            builder.Register<WindowProvider>(Lifetime.Scoped);
            builder.Register<WindowNavigator>(Lifetime.Scoped);
            builder.Register<UnityLocalizationService>(Lifetime.Singleton).As<ILocalizationService>();

            builder.RegisterInstance<IDataLoader<RankCatalog>>(_rankCatalogConfig);
            builder.RegisterInstance<IDataLoader<CardCatalog>>(_cardCatalogConfig);
            builder.RegisterInstance<IDataLoader<RankPromotionCatalog>>(_rankPromotionCatalogConfig);
            builder.RegisterInstance<IDataLoader<RewardBundleCatalog>>(_rewardBundleCatalogConfig);
            builder.RegisterInstance<IDataLoader<IconCatalog<ItemId>>>(_itemIconCatalogConfig);
            builder.RegisterInstance<IDataLoader<IconCatalog<CardId>>>(
                _cardIconCatalogConfig);
            builder.RegisterInstance<IUserDefaultsProvider>(_userDefaultsConfig);

            builder.Register(
                resolver => resolver.Resolve<IDataLoader<CardCatalog>>().Load(),
                Lifetime.Singleton);
            builder.Register(
                resolver => resolver.Resolve<IDataLoader<RankPromotionCatalog>>().Load(),
                Lifetime.Singleton);
            builder.Register(
                resolver => resolver.Resolve<IDataLoader<RewardBundleCatalog>>().Load(),
                Lifetime.Singleton);
            builder
                .Register(
                    resolver => resolver
                        .Resolve<IDataLoader<IconCatalog<ItemId>>>()
                        .Load(),
                    Lifetime.Singleton)
                .As<IIconProvider<ItemId>>();
            builder
                .Register(
                    resolver => resolver
                        .Resolve<IDataLoader<IconCatalog<CardId>>>()
                        .Load(),
                    Lifetime.Singleton)
                .As<IIconProvider<CardId>>();
            builder.Register<JsonFileSaveService>(Lifetime.Singleton).As<ISaveService>();
            builder.Register<UserInitialDataLoader>(Lifetime.Singleton).As<IDataLoader<UserInitialData>>();

            builder.Register(resolver => resolver.Resolve<IDataLoader<UserInitialData>>().Load(), Lifetime.Singleton);
            builder.Register(resolver => resolver.Resolve<UserInitialData>().Identity, Lifetime.Singleton);
            builder.Register(resolver => resolver.Resolve<UserInitialData>().Items, Lifetime.Singleton);
            builder.Register(resolver => resolver.Resolve<UserInitialData>().Progress, Lifetime.Singleton);
            builder.Register(resolver => resolver.Resolve<UserInitialData>().PromotionOrder, Lifetime.Singleton);
            builder.Register(resolver => resolver.Resolve<UserInitialData>().Cards, Lifetime.Singleton);
            builder.Register(resolver => resolver.Resolve<UserInitialData>().RewardClaims, Lifetime.Singleton);

            builder.Register<UserItems>(Lifetime.Singleton).As<IUserItems>();
            builder.Register<UserCards>(Lifetime.Singleton).As<IUserCards>();
            builder.Register<UserPromotionOrder>(Lifetime.Singleton).As<IUserPromotionOrder>();
            builder.Register<UserRewardClaims>(Lifetime.Singleton).As<IUserRewardClaims>();
            builder.Register<RankProgression>(Lifetime.Singleton).As<IRankProgression>();
            builder.Register<UserProgress>(Lifetime.Singleton).As<IUserProgress>();
            builder.Register<CardExperienceService>(Lifetime.Singleton).As<ICardExperienceService>();
            builder.Register<RewardService>(Lifetime.Singleton).As<IRewardService>();
            builder.Register<RewardGrantService>(Lifetime.Singleton).As<IRewardGrantService>();
            builder.Register<RewardIconProvider>(Lifetime.Singleton);

            builder.RegisterEntryPoint<UserSaveController>();
        }
    }
}