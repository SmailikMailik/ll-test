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

        private readonly IReadOnlyList<IBootstrapOperation> _operations;
        private readonly SceneLoadingBootstrapOperation _sceneLoading;
        private readonly ProgressBar _progressBar;

        private bool _activateSceneOnNextTick;
        private bool _isCompleted;
        private bool _isStarted;

        [Inject]
        internal BootstrapFlow(
            IReadOnlyList<IBootstrapOperation> operations,
            SceneLoadingBootstrapOperation sceneLoading,
            ProgressBar progressBar)
        {
            _operations = operations ?? throw new ArgumentNullException(nameof(operations));
            _sceneLoading = sceneLoading ?? throw new ArgumentNullException(nameof(sceneLoading));
            _progressBar = progressBar ?? throw new ArgumentNullException(nameof(progressBar));
        }

        public void Start()
        {
            _progressBar.SetProgress(MinProgress);

            foreach (var operation in _operations)
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

            _progressBar.SetProgress(CalculateProgress());

            if (AreReady() is false)
                return;

            _progressBar.SetProgress(MaxProgress);
            _activateSceneOnNextTick = true;
        }

        private float CalculateProgress()
        {
            var progress = MaxProgress;

            foreach (var operation in _operations)
                progress = Mathf.Min(progress, operation.Progress);

            return progress;
        }

        private bool AreReady()
        {
            foreach (var operation in _operations)
            {
                if (operation.IsReady is false)
                    return false;
            }

            return true;
        }

        private void EnsureSucceeded()
        {
            foreach (var operation in _operations)
                operation.EnsureSucceeded();
        }
    }
}