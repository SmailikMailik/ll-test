using System;
using LL.UI.Windows;
using LL.UI.Windows.Configuration;
using VContainer;
using VContainer.Unity;

namespace LL.Composition.Installers
{
    internal sealed class WindowInstaller : IInstaller
    {
        private readonly WindowCatalogConfig _windowCatalogConfig;

        internal WindowInstaller(WindowCatalogConfig windowCatalogConfig)
        {
            _windowCatalogConfig = windowCatalogConfig ?? throw new ArgumentNullException(nameof(windowCatalogConfig));
        }

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterLoadedData(_windowCatalogConfig);
            builder.Register<WindowProvider>(Lifetime.Scoped);
            builder.Register<WindowNavigator>(Lifetime.Scoped);
        }
    }
}