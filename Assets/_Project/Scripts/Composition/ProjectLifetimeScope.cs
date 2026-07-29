using System;
using LL.Composition.Persistence;
using LL.Game.Cards.Configuration;
using LL.Game.Payments;
using LL.Game.Promotions.Configuration;
using LL.Game.Ranks.Configuration;
using LL.Game.Ranks;
using LL.Game.Rewards.Configuration;
using LL.Game.Rewards.Services;
using LL.Game.Upgrades;
using LL.Infrastructure.Loading;
using LL.Infrastructure.Saving;
using LL.Infrastructure.Validation;
using LL.Presentation.Icons.Configuration;
using LL.Presentation.Icons;
using LL.Presentation.Localization;
using LL.UI.Windows.Configuration;
using LL.UI.Windows;
using LL.User.Configuration;
using LL.User.Persistence;
using LL.User.Snapshots;
using LL.User.State.Items;
using LL.User.State.Progress;
using LL.User.State.Promotions;
using LL.User.State.Rewards;
using LL.Validation.Reporting;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer.Unity;
using VContainer;

namespace LL.Composition
{
    [DisallowMultipleComponent]
    internal sealed class ProjectLifetimeScope : LifetimeScope
    {
        [SerializeField] private WindowCatalog _windowCatalog;

        [Header("Game Data")]
        [SerializeField] private RankCatalogConfig _rankCatalogConfig;
        [SerializeField] private CardCatalogConfig _cardCatalogConfig;
        [SerializeField] private RankPromotionCatalogConfig _rankPromotionCatalogConfig;

        [FormerlySerializedAs("_rewardBundleCatalogConfig")]
        [SerializeField] private RewardCatalogConfig _rewardCatalogConfig;

        [Header("Presentation")]
        [SerializeField] private ItemIconCatalogConfig _itemIconCatalogConfig;

        [Header("User")]
        [SerializeField] private UserDefaultsConfig _userDefaultsConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterWindows(builder);
            RegisterLocalization(builder);
            RegisterValidationReporting(builder);
            RegisterGameData(builder);
            RegisterPresentation(builder);
            RegisterUserPersistence(builder);
            RegisterUserState(builder);
            RegisterGameServices(builder);
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

        private static void RegisterValidationReporting(IContainerBuilder builder)
        {
            builder.RegisterInstance<IValidationIssueFormatter>(new ValidationIssueFormatter());
            builder.Register<UnityConsoleValidationReporter>(Lifetime.Singleton).As<IValidationReporter>();
        }

        private void RegisterGameData(IContainerBuilder builder)
        {
            RegisterLoadedData(builder, _rankCatalogConfig);
            RegisterLoadedData(builder, _cardCatalogConfig);
            RegisterLoadedData(builder, _rankPromotionCatalogConfig);
            RegisterLoadedData(builder, _rewardCatalogConfig);
        }

        private void RegisterPresentation(IContainerBuilder builder)
        {
            RegisterLoadedData(builder, _itemIconCatalogConfig);
        }

        private void RegisterUserPersistence(IContainerBuilder builder)
        {
            builder.RegisterInstance<IUserDefaultsProvider>(_userDefaultsConfig);
            builder.RegisterInstance<ISaveService>(PersistenceComposition.CreateDefaultSaveService());
            builder.Register<UserSnapshotLoader>(Lifetime.Singleton).As<IDataLoader<UserSnapshot>>();

            RegisterLoadedData<UserSnapshot>(builder);
            RegisterUserSnapshotPart(builder, snapshot => snapshot.Identity);
            RegisterUserSnapshotPart(builder, snapshot => snapshot.Items);
            RegisterUserSnapshotPart(builder, snapshot => snapshot.Progress);
            RegisterUserSnapshotPart(builder, snapshot => snapshot.PromotionOrder);
            RegisterUserSnapshotPart(builder, snapshot => snapshot.RewardClaims);

            builder.RegisterEntryPoint<UserSaveController>();
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
            builder.Register<RewardGrantService>(Lifetime.Singleton).As<IRewardGrantService>();
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

        private static void RegisterUserSnapshotPart<TData>(
            IContainerBuilder builder,
            Func<UserSnapshot, TData> selector)
        {
            builder.Register(resolver => selector(resolver.Resolve<UserSnapshot>()), Lifetime.Singleton);
        }
    }
}