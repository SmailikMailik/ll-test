using LL.Game.Promotions.Services;
using LL.Game.Upgrades;
using LL.Presentation.Quests;
using LL.Presentation.Upgrades;
using LL.UI.Promotions;
using LL.UI.Windows;
using LL.UI.Windows.Flows;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace LL.Composition.Scopes
{
    [DisallowMultipleComponent]
    internal sealed class MainLifetimeScope : LifetimeScope
    {
        [SerializeField] private WindowController _windowController;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterWindow(builder);
            RegisterConfirmations(builder);
            RegisterPromotion(builder);
            RegisterUpgradeFlow(builder);
        }

        private void RegisterWindow(IContainerBuilder builder)
        {
            builder.RegisterComponent(_windowController);
        }

        private static void RegisterConfirmations(IContainerBuilder builder)
        {
            builder
                .Register<ModalQuestCompletionConfirmation>(Lifetime.Scoped)
                .As<IQuestCompletionConfirmation>();
            builder
                .Register<ModalRankPromotionConfirmation>(Lifetime.Scoped)
                .As<IRankPromotionConfirmation>();
            builder
                .Register<ModalExperienceOverflowConfirmation>(Lifetime.Scoped)
                .As<IExperienceOverflowConfirmation>();
        }

        private static void RegisterPromotion(IContainerBuilder builder)
        {
            builder.Register<RankPromotionService>(Lifetime.Scoped).As<IRankPromotionService>();
            builder.Register<RankPromotionFlow>(Lifetime.Scoped);
        }

        private static void RegisterUpgradeFlow(IContainerBuilder builder)
        {
            builder.Register<UpgradeFlow>(Lifetime.Scoped);
            builder.RegisterEntryPoint<UpgradeFlowStartup>();
        }
    }
}