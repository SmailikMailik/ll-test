using DG.Tweening;
using UnityEngine;

namespace LL.UI.Windows.Views.Reward
{
    [DisallowMultipleComponent]
    internal sealed class RotationAnimation : RewardAnimation
    {
        [SerializeField] private RectTransform _target;

        [Min(0.01f)]
        [SerializeField] private float _durationSeconds = 20f;

        private const float FullRotationDegrees = 360f;

        protected override void BuildSequence(Sequence sequence)
        {
            sequence
                .Append(
                    _target
                        .DOLocalRotate(
                            Vector3.back * FullRotationDegrees,
                            _durationSeconds,
                            RotateMode.FastBeyond360)
                        .SetEase(Ease.Linear))
                .SetLoops(-1, LoopType.Restart);
        }

        protected override void ApplyHiddenState()
        {
            _target.localRotation = Quaternion.identity;
        }
    }
}