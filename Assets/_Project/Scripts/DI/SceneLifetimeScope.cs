using LL.UI.Windows;
using LL.UI.Windows.View.Upgrade;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace LL.DI
{
    internal sealed class SceneLifetimeScope : LifetimeScope
    {
        [SerializeField] private WindowsController _windowController;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_windowController);
        }

        private void Start()
        {
            _windowController.Show(WindowType.Upgrade, new UpgradeParameters(1));
        }
    }
}