using LL.Composition.Installers;
using LL.Game.Cards.Configuration;
using LL.Game.Payments;
using LL.Game.Promotions.Configuration;
using LL.Game.Quests.Configuration;
using LL.Game.Ranks.Configuration;
using LL.Game.Rewards.Configuration;
using LL.Game.Rewards.Services;
using LL.Game.Upgrades;
using LL.Infrastructure.Validation;
using LL.Presentation.Icons.Configuration;
using LL.Presentation.Localization;
using LL.UI.Windows.Configuration;
using LL.User.Configuration;
using LL.Validation.Reporting;
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
        [SerializeField] private RankCatalogConfig _rankCatalogConfig;
        [SerializeField] private CardCatalogConfig _cardCatalogConfig;
        [SerializeField] private QuestCatalogConfig _questCatalogConfig;
        [SerializeField] private RankPromotionCatalogConfig _rankPromotionCatalogConfig;
        [SerializeField] private RewardCatalogConfig _rewardCatalogConfig;

        [Header("Presentation")]
        [SerializeField] private ItemIconCatalogConfig _itemIconCatalogConfig;

        [Header("User")]
        [SerializeField] private UserDefaultsConfig _userDefaultsConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            new WindowInstaller(_windowCatalogConfig).Install(builder);
            RegisterLocalization(builder);
            RegisterValidationReporting(builder);
            new GameDataInstaller(
                    _rankCatalogConfig,
                    _cardCatalogConfig,
                    _questCatalogConfig,
                    _rankPromotionCatalogConfig,
                    _rewardCatalogConfig)
                .Install(builder);
            RegisterPresentation(builder);
            new UserInstaller(_userDefaultsConfig).Install(builder);
            RegisterGameServices(builder);
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

        private void RegisterPresentation(IContainerBuilder builder)
        {
            builder.RegisterLoadedData(_itemIconCatalogConfig);
        }

        private static void RegisterGameServices(IContainerBuilder builder)
        {
            builder.Register<PaymentService>(Lifetime.Singleton).As<IPaymentService>();
            builder.Register<CardExperienceService>(Lifetime.Singleton).As<ICardExperienceService>();
            builder.Register<RewardGrantService>(Lifetime.Singleton).As<IRewardGrantService>();
        }
    }
}