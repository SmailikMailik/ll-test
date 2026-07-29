using System;
using LL.Game.Cards.Configuration;
using LL.Game.Promotions.Configuration;
using LL.Game.Ranks;
using LL.Game.Ranks.Configuration;
using LL.Identifiers;
using LL.Loading;
using LL.Payments;
using LL.Presentation.Configuration;
using LL.Presentation.Icons;
using LL.Presentation.Localization;
using LL.Rewards.Configuration;
using LL.Rewards.Services;
using LL.Saving;
using LL.UI.Windows;
using LL.UI.Windows.Configuration;
using LL.Upgrades;
using LL.User.Configuration;
using LL.User.Core;
using LL.User.Core.Items;
using LL.User.Core.Progress;
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

        [Header("User")]
        [SerializeField] private UserDefaultsConfig _userDefaultsConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterWindows(builder);
            RegisterLocalization(builder);
            RegisterCatalogs(builder);
            RegisterPersistence(builder);
            RegisterUserState(builder);
            RegisterGameServices(builder);

            builder.RegisterEntryPoint<UserSaveController>();
        }

        private void RegisterWindows(IContainerBuilder builder)
        {
            builder.RegisterInstance(_windowCatalog);
            builder.Register<WindowProvider>(Lifetime.Scoped);
            builder.Register<WindowNavigator>(Lifetime.Scoped);
        }

        private static void RegisterLocalization(IContainerBuilder builder)
        {
            builder.Register<UnityLocalizationService>(Lifetime.Singleton).As<ILocalizationService>();
        }

        private void RegisterCatalogs(IContainerBuilder builder)
        {
            RegisterLoadedData(builder, _rankCatalogConfig);
            RegisterLoadedData(builder, _cardCatalogConfig);
            RegisterLoadedData(builder, _rankPromotionCatalogConfig);
            RegisterLoadedData(builder, _rewardBundleCatalogConfig);
            RegisterIconCatalog(builder, _itemIconCatalogConfig);
        }

        private void RegisterPersistence(IContainerBuilder builder)
        {
            builder.RegisterInstance<IUserDefaultsProvider>(_userDefaultsConfig);
            builder.Register<JsonFileSaveService>(Lifetime.Singleton).As<ISaveService>();
            builder.Register<UserDataLoader>(Lifetime.Singleton).As<IDataLoader<UserData>>();

            RegisterLoadedData<UserData>(builder);
            RegisterUserDataPart(builder, userData => userData.Identity);
            RegisterUserDataPart(builder, userData => userData.Items);
            RegisterUserDataPart(builder, userData => userData.Progress);
            RegisterUserDataPart(builder, userData => userData.PromotionOrder);
            RegisterUserDataPart(builder, userData => userData.RewardClaims);
        }

        private static void RegisterUserState(IContainerBuilder builder)
        {
            builder.Register<UserItems>(Lifetime.Singleton).As<IUserItems>();
            builder.Register<UserPromotionOrder>(Lifetime.Singleton).As<IUserPromotionOrder>();
            builder.Register<UserRewardClaims>(Lifetime.Singleton).As<IUserRewardClaims>();
            builder.Register<RankProgression>(Lifetime.Singleton).As<IRankProgression>();
            builder.Register<UserProgress>(Lifetime.Singleton).As<IUserProgress>();
        }

        private static void RegisterGameServices(IContainerBuilder builder)
        {
            builder.Register<PaymentService>(Lifetime.Singleton).As<IPaymentService>();
            builder.Register<CardExperienceService>(Lifetime.Singleton).As<ICardExperienceService>();
            builder.Register<RewardService>(Lifetime.Singleton).As<IRewardService>();
            builder.Register<RewardGrantService>(Lifetime.Singleton).As<IRewardGrantService>();
            builder.Register<RewardIconProvider>(Lifetime.Singleton);
        }

        private static void RegisterLoadedData<TData>(IContainerBuilder builder, IDataLoader<TData> loader)
        {
            builder.RegisterInstance(loader);
            RegisterLoadedData<TData>(builder);
        }

        private static void RegisterLoadedData<TData>(IContainerBuilder builder)
        {
            builder.Register(resolver => resolver.Resolve<IDataLoader<TData>>().Load(), Lifetime.Singleton);
        }

        private static void RegisterIconCatalog<TId>(
            IContainerBuilder builder,
            IDataLoader<IconCatalog<TId>> loader)
            where TId : struct, IIdentifier
        {
            builder.RegisterInstance(loader);
            builder
                .Register(resolver => resolver.Resolve<IDataLoader<IconCatalog<TId>>>().Load(), Lifetime.Singleton)
                .As<IIconProvider<TId>>();
        }

        private static void RegisterUserDataPart<TData>(
            IContainerBuilder builder,
            Func<UserData, TData> selector)
        {
            builder.Register(resolver => selector(resolver.Resolve<UserData>()), Lifetime.Singleton);
        }
    }
}