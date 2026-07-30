using System;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace LL.Bootstrap
{
    internal sealed class BootstrapLoadingOperation
    {
        private const float MaxProgress = 1f;
        private const float SceneReadyProgress = 0.9f;
        private const float MinAllowedDisplaySeconds = 0f;

        private readonly AsyncOperationHandle<LocalizationSettings> _localizationOperation;
        private readonly AsyncOperation _sceneOperation;
        private readonly float _minDisplaySeconds;
        private readonly float _startedAt;

        internal float Progress =>
            Mathf.Min(InitializationProgress, TimeProgress);

        internal bool IsReady =>
            SceneProgress >= MaxProgress &&
            _localizationOperation.IsDone &&
            TimeProgress >= MaxProgress;

        private float InitializationProgress =>
            Mathf.Min(SceneProgress, _localizationOperation.PercentComplete);

        private float SceneProgress =>
            Mathf.Clamp01(_sceneOperation.progress / SceneReadyProgress);

        private float TimeProgress => _minDisplaySeconds > MinAllowedDisplaySeconds
            ? Mathf.Clamp01((Time.realtimeSinceStartup - _startedAt) / _minDisplaySeconds)
            : MaxProgress;

        internal BootstrapLoadingOperation(string sceneName, float minDisplaySeconds)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
                throw new ArgumentException("Scene name cannot be empty.", nameof(sceneName));

            if (minDisplaySeconds < MinAllowedDisplaySeconds)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(minDisplaySeconds),
                    minDisplaySeconds,
                    "Minimum display time cannot be negative.");
            }

            _minDisplaySeconds = minDisplaySeconds;
            _startedAt = Time.realtimeSinceStartup;
            _localizationOperation = LocalizationSettings.InitializationOperation;
            _sceneOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

            if (_sceneOperation == null)
                throw new InvalidOperationException($"Failed to start loading scene '{sceneName}'.");

            _sceneOperation.allowSceneActivation = false;
        }

        internal void ActivateScene()
        {
            EnsureSucceeded();

            if (IsReady is false)
                throw new InvalidOperationException("Bootstrap loading operation is not ready.");

            _sceneOperation.allowSceneActivation = true;
        }

        internal void EnsureSucceeded()
        {
            if (_localizationOperation is { IsDone: true, Status: AsyncOperationStatus.Failed })
            {
                throw new InvalidOperationException(
                    "Failed to initialize localization.",
                    _localizationOperation.OperationException);
            }
        }
    }
}