using LL.Purchasing;
using LL.UI.Purchasing;
using LL.UI.Windows;
using LL.UI.Windows.Views.Upgrade;
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
            builder.Register<DemoPurchaseConfirmation>(Lifetime.Scoped).As<IPurchaseConfirmation>();
            builder.Register<PurchaseService>(Lifetime.Scoped).As<IPurchaseService>();
        }

        private void Start()
        {
            _windowController.Show(new UpgradeWindowParameters());
        }
    }
}