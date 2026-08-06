using System;
using System.Collections.Generic;
using LL.UI.Controls;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace LL.Bootstrap
{
    internal sealed class BootstrapFlow : IStartable, ITickable
    {
        private const float MinProgress = 0f;
        private const float MaxProgress = 1f;

        private readonly IReadOnlyList<IBootstrapOperation> _prerequisiteOperations;
        private readonly LocalizationBootstrapOperation _localization;
        private readonly SceneLoadingBootstrapOperation _sceneLoading;
        private readonly ProgressBar _progressBar;

        private bool _activateSceneOnNextTick;
        private bool _isCompleted;
        private bool _isSceneLoadingStarted;
        private bool _isStarted;

        [Inject]
        internal BootstrapFlow(
            IReadOnlyList<IBootstrapOperation> prerequisiteOperations,
            LocalizationBootstrapOperation localization,
            SceneLoadingBootstrapOperation sceneLoading,
            ProgressBar progressBar)
        {
            _prerequisiteOperations = prerequisiteOperations ?? throw new ArgumentNullException(nameof(prerequisiteOperations));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _sceneLoading = sceneLoading ?? throw new ArgumentNullException(nameof(sceneLoading));
            _progressBar = progressBar ?? throw new ArgumentNullException(nameof(progressBar));
        }

        public void Start()
        {
            _progressBar.SetProgress(MinProgress);

            foreach (var operation in _prerequisiteOperations)
                operation.Start();

            _isStarted = true;
        }

        public void Tick()
        {
            if (_isStarted is false || _isCompleted)
                return;

            if (_activateSceneOnNextTick)
            {
                _isCompleted = true;
                _sceneLoading.Activate();
                return;
            }

            try
            {
                EnsureSucceeded();
            }
            catch
            {
                _isCompleted = true;
                throw;
            }

            if (_isSceneLoadingStarted is false && _localization.IsReady)
            {
                _sceneLoading.Start();
                _isSceneLoadingStarted = true;
            }

            _progressBar.SetProgress(CalculateProgress());

            if (AreReady() is false)
                return;

            _progressBar.SetProgress(MaxProgress);
            _activateSceneOnNextTick = true;
        }

        private float CalculateProgress()
        {
            var progress = MaxProgress;

            foreach (var operation in _prerequisiteOperations)
                progress = Mathf.Min(progress, operation.Progress);

            if (_isSceneLoadingStarted)
                progress = Mathf.Min(progress, _sceneLoading.Progress);

            return progress;
        }

        private bool AreReady()
        {
            foreach (var operation in _prerequisiteOperations)
            {
                if (operation.IsReady is false)
                    return false;
            }

            return _isSceneLoadingStarted && _sceneLoading.IsReady;
        }

        private void EnsureSucceeded()
        {
            foreach (var operation in _prerequisiteOperations)
                operation.EnsureSucceeded();
        }
    }
}