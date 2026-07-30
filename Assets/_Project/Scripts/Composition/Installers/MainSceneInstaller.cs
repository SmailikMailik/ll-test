using System;
using LL.Game.Promotions.Services;
using LL.Presentation.Promotions;
using LL.Presentation.Quests;
using LL.Presentation.Upgrades;
using LL.UI.Windows;
using LL.UI.Windows.Flows;
using LL.UI.Windows.Modal;
using VContainer;
using VContainer.Unity;

namespace LL.Composition.Installers
{
    internal sealed class MainSceneInstaller : IInstaller
    {
        private readonly WindowController _windowController;

        internal MainSceneInstaller(WindowController windowController)
        {
            _windowController = windowController ?? throw new ArgumentNullException(nameof(windowController));
        }

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterComponent(_windowController);
            RegisterConfirmations(builder);
            RegisterPromotion(builder);
            RegisterUpgradeFlow(builder);
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