using LL.Composition.Installers;
using LL.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace LL.Composition.Scopes
{
    [DisallowMultipleComponent]
    internal sealed class BootstrapLifetimeScope : LifetimeScope
    {
        [SerializeField] private ProgressBar _progressBar;
        [SerializeField, Min(0f)] private float _minDisplaySeconds = 0.75f;

        protected override void Configure(IContainerBuilder builder)
        {
            new BootstrapInstaller(_progressBar, _minDisplaySeconds).Install(builder);
        }
    }
}