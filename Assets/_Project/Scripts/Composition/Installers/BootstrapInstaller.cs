using System;
using LL.Bootstrap;
using LL.UI;
using VContainer;
using VContainer.Unity;

namespace LL.Composition.Installers
{
    internal sealed class BootstrapInstaller : IInstaller
    {
        private const string TargetSceneName = "Main";

        private readonly ProgressBar _progressBar;
        private readonly float _minDisplaySeconds;

        internal BootstrapInstaller(ProgressBar progressBar, float minDisplaySeconds)
        {
            if (progressBar == null)
                throw new ArgumentNullException(nameof(progressBar));

            _progressBar = progressBar;
            _minDisplaySeconds = minDisplaySeconds;
        }

        public void Install(IContainerBuilder builder)
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