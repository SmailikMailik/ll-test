using LL.Presentation.Purchasing;
using LL.Presentation.Upgrades;
using LL.Purchasing;
using LL.Upgrades;
using LL.UI.Windows;
using LL.UI.Windows.Flows;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace LL.DI
{
    internal sealed class SceneLifetimeScope : LifetimeScope
    {
        [SerializeField] private WindowController _windowController;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_windowController);
            builder.Register<ModalPurchaseConfirmation>(Lifetime.Scoped).As<IPurchaseConfirmation>();
            builder.Register<ModalExperienceOverflowConfirmation>(Lifetime.Scoped).As<IExperienceOverflowConfirmation>();
            builder.Register<PurchaseService>(Lifetime.Scoped).As<IPurchaseService>();
            builder.Register<UpgradeFlow>(Lifetime.Scoped);
            builder.RegisterEntryPoint<UpgradeFlowStartup>();
        }
    }
}