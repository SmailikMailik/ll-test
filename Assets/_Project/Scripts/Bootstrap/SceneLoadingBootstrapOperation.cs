using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace LL.Bootstrap
{
    internal sealed class SceneLoadingBootstrapOperation : IBootstrapOperation
    {
        private const float MinProgress = 0f;
        private const float MaxProgress = 1f;
        private const float SceneReadyProgress = 0.9f;

        private readonly string _sceneName;

        private AsyncOperation _operation;

        public float Progress => _operation is null
            ? MinProgress
            : Mathf.Clamp01(_operation.progress / SceneReadyProgress);

        public bool IsReady => Progress >= MaxProgress;

        [Inject]
        internal SceneLoadingBootstrapOperation(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
                throw new ArgumentException("Scene name cannot be empty.", nameof(sceneName));

            _sceneName = sceneName;
        }

        public void Start()
        {
            _operation = SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Single);

            if (_operation is null)
                throw new InvalidOperationException($"Failed to start loading scene '{_sceneName}'.");

            _operation.allowSceneActivation = false;
        }

        public void Activate()
        {
            if (IsReady is false)
                throw new InvalidOperationException("Scene loading operation is not ready.");

            _operation.allowSceneActivation = true;
        }

        public void EnsureSucceeded() { }
    }
}