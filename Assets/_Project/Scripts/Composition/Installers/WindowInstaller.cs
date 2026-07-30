using System;
using LL.UI.Windows;
using LL.UI.Windows.Configuration;
using VContainer;
using VContainer.Unity;

namespace LL.Composition.Installers
{
    internal sealed class WindowInstaller : IInstaller
    {
        private readonly WindowCatalog _windowCatalog;

        internal WindowInstaller(WindowCatalog windowCatalog)
        {
            _windowCatalog = windowCatalog ?? throw new ArgumentNullException(nameof(windowCatalog));
        }

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(_windowCatalog);
            builder.Register<WindowProvider>(Lifetime.Scoped);
            builder.Register<WindowNavigator>(Lifetime.Scoped);
        }
    }
}