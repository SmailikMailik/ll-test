using LL.Bootstrap;
using LL.UI.Controls;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace LL.Composition.Scopes
{
    [DisallowMultipleComponent]
    internal sealed class BootstrapLifetimeScope : LifetimeScope
    {
        [SerializeField] private ProgressBar _progressBar;

        [Min(0f)]
        [SerializeField] private float _minDisplaySeconds = 0.75f;

        private const string TargetSceneName = "Main";

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_progressBar);
            builder
                .Register<LocalizationBootstrapOperation>(Lifetime.Scoped)
                .As<IBootstrapOperation>();
            builder
                .Register<MinDisplayBootstrapOperation>(Lifetime.Scoped)
                .As<IBootstrapOperation>()
                .WithParameter(_minDisplaySeconds);
            builder
                .Register<SceneLoadingBootstrapOperation>(Lifetime.Scoped)
                .AsSelf()
                .As<IBootstrapOperation>()
                .WithParameter(TargetSceneName);
            builder.RegisterEntryPoint<BootstrapFlow>(Lifetime.Scoped);
        }
    }
}