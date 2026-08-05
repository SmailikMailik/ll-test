using System;
using LL.Infrastructure.Loading;
using LL.UI.Windows;
using VContainer;
using VContainer.Unity;

namespace LL.Composition.Installers
{
    internal sealed class WindowInstaller : IInstaller
    {
        private readonly IDataLoader<WindowCatalog> _windowCatalogLoader;

        internal WindowInstaller(IDataLoader<WindowCatalog> windowCatalogLoader)
        {
            _windowCatalogLoader = windowCatalogLoader ?? throw new ArgumentNullException(nameof(windowCatalogLoader));
        }

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterLoadedData(_windowCatalogLoader);
            builder.Register<WindowProvider>(Lifetime.Scoped);
            builder.Register<WindowNavigator>(Lifetime.Scoped);
        }
    }
}