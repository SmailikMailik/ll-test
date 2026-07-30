using System;
using UnityEngine;
using VContainer;

namespace LL.Bootstrap
{
    internal sealed class MinDisplayBootstrapOperation : IBootstrapOperation
    {
        private const float MinAllowedDisplaySeconds = 0f;
        private const float MaxProgress = 1f;

        private readonly float _minDisplaySeconds;

        private float _startedAt;

        public float Progress => _minDisplaySeconds > MinAllowedDisplaySeconds
            ? Mathf.Clamp01((Time.realtimeSinceStartup - _startedAt) / _minDisplaySeconds)
            : MaxProgress;

        public bool IsReady => Progress >= MaxProgress;

        [Inject]
        internal MinDisplayBootstrapOperation(float minDisplaySeconds)
        {
            if (minDisplaySeconds < MinAllowedDisplaySeconds)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(minDisplaySeconds),
                    minDisplaySeconds,
                    "Minimum display time cannot be negative.");
            }

            _minDisplaySeconds = minDisplaySeconds;
        }

        public void Start()
        {
            _startedAt = Time.realtimeSinceStartup;
        }

        public void EnsureSucceeded() { }
    }
}