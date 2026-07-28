using LL.Presentation.Orders;
using LL.Presentation.Promotions;
using LL.Presentation.Upgrades;
using LL.Promotions;
using LL.Purchasing;
using LL.Upgrades;
using LL.UI.Windows;
using LL.UI.Windows.Flows;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace LL.DI
{
    [DisallowMultipleComponent]
    internal sealed class SceneLifetimeScope : LifetimeScope
    {
        [SerializeField] private WindowController _windowController;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_windowController);
            builder
                .Register<ModalOrderCompletionConfirmation>(Lifetime.Scoped)
                .As<IOrderCompletionConfirmation>();
            builder.Register<ModalRankPromotionConfirmation>(Lifetime.Scoped).As<IPurchaseConfirmation>();
            builder
                .Register<ModalExperienceOverflowConfirmation>(Lifetime.Scoped)
                .As<IExperienceOverflowConfirmation>();
            builder.Register<PurchaseService>(Lifetime.Scoped).As<IPurchaseService>();
            builder.Register<RankPromotionService>(Lifetime.Scoped).As<IRankPromotionService>();
            builder.Register<UpgradeFlow>(Lifetime.Scoped);
            builder.RegisterEntryPoint<UpgradeFlowStartup>();
        }
    }
}