using System;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;

namespace LL.Bootstrap
{
    internal sealed class LocalizationBootstrapOperation : IBootstrapOperation
    {
        private const float MinProgress = 0f;

        private AsyncOperationHandle<LocalizationSettings> _operation;
        private bool _isStarted;

        public float Progress => _isStarted
            ? _operation.PercentComplete
            : MinProgress;

        public bool IsReady => _isStarted && _operation.IsDone;

        [Inject]
        internal LocalizationBootstrapOperation() { }

        public void Start()
        {
            _operation = LocalizationSettings.InitializationOperation;
            _isStarted = true;
        }

        public void EnsureSucceeded()
        {
            if (_operation is { IsDone: true, Status: AsyncOperationStatus.Failed })
            {
                throw new InvalidOperationException(
                    "Failed to initialize localization.",
                    _operation.OperationException);
            }
        }
    }
}