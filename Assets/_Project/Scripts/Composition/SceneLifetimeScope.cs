using LL.Presentation.Orders;
using LL.Presentation.Promotions;
using LL.Presentation.Upgrades;
using LL.Game.Promotions.Services;
using LL.Game.Upgrades;
using LL.UI.Windows;
using LL.UI.Windows.Flows;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace LL.Composition
{
    [DisallowMultipleComponent]
    internal sealed class SceneLifetimeScope : LifetimeScope
    {
        [SerializeField] private WindowController _windowController;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterWindow(builder);
            RegisterConfirmations(builder);
            RegisterPromotionServices(builder);
            RegisterUpgradeFlow(builder);
        }

        private void RegisterWindow(IContainerBuilder builder)
        {
            builder.RegisterComponent(_windowController);
        }

        private static void RegisterConfirmations(IContainerBuilder builder)
        {
            builder
                .Register<ModalOrderCompletionConfirmation>(Lifetime.Scoped)
                .As<IOrderCompletionConfirmation>();
            builder
                .Register<ModalRankPromotionConfirmation>(Lifetime.Scoped)
                .As<IRankPromotionConfirmation>();
            builder
                .Register<ModalExperienceOverflowConfirmation>(Lifetime.Scoped)
                .As<IExperienceOverflowConfirmation>();
        }

        private static void RegisterPromotionServices(IContainerBuilder builder)
        {
            builder.Register<RankPromotionService>(Lifetime.Scoped).As<IRankPromotionService>();
        }

        private static void RegisterUpgradeFlow(IContainerBuilder builder)
        {
            builder.Register<UpgradeFlow>(Lifetime.Scoped);
            builder.RegisterEntryPoint<UpgradeFlowStartup>();
        }
    }
}