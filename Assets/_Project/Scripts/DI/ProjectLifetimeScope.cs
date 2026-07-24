using LL.UI.Windows;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace LL.DI
{
    internal sealed class ProjectLifetimeScope : LifetimeScope
    {
        [SerializeField] private WindowsSettings _windowsSettings;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_windowsSettings);
            builder.Register<WindowsFactory>(Lifetime.Scoped);

            builder.Register<UserData>(Lifetime.Singleton);
        }
    }
}