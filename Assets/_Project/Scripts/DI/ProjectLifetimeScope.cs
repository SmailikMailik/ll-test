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

        [Header("User Data Defaults")]
        [SerializeField, Min(0)] private int _defaultSoftAmount = 99;
        [SerializeField, Min(0)] private int _defaultHardAmount = 99;
        [SerializeField, Min(0)] private int _defaultMasterPointAmount = 99;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_windowCatalog);
            builder.Register<WindowProvider>(Lifetime.Scoped);
            builder.Register<WindowNavigator>(Lifetime.Scoped);

            builder.Register(
                _ => new UserData(
                    _defaultSoftAmount,
                    _defaultHardAmount,
                    _defaultMasterPointAmount),
                Lifetime.Singleton);
        }
    }
}