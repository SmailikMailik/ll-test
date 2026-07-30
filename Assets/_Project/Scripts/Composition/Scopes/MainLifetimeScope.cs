using LL.Composition.Installers;
using LL.UI.Windows;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace LL.Composition.Scopes
{
    [DisallowMultipleComponent]
    internal sealed class MainLifetimeScope : LifetimeScope
    {
        [SerializeField] private WindowController _windowController;

        protected override void Configure(IContainerBuilder builder)
        {
            new MainSceneInstaller(_windowController).Install(builder);
        }
    }
}