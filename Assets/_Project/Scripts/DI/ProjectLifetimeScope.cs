using LL.UI.Windows;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

namespace LL.DI
{
    internal sealed class ProjectLifetimeScope : LifetimeScope
    {
        [FormerlySerializedAs("_windowsSettings")]
        [SerializeField] private WindowCatalog _windowCatalog;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_windowCatalog);
            builder.Register<WindowProvider>(Lifetime.Scoped);
            builder.Register<WindowNavigator>(Lifetime.Scoped);

            builder.Register<UserData>(Lifetime.Singleton);
        }
    }
}