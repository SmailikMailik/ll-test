using DG.Tweening;
using UnityEngine;

namespace LL.UI.Windows.Views.Reward
{
    [DisallowMultipleComponent]
    internal sealed class RotationAnimation : RewardAnimation
    {
        [SerializeField] private RectTransform _target;
        [SerializeField, Min(0.01f)] private float _durationSeconds = 20f;

        private const float FullRotationDegrees = 360f;

        protected override void BuildSequence(Sequence sequence)
        {
            var fullRotation = Vector3.back * FullRotationDegrees;
            var rotationTween = _target
                .DOLocalRotate(fullRotation, _durationSeconds, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear);

            sequence.Append(rotationTween);
            sequence.SetLoops(-1, LoopType.Restart);
        }

        protected override void ApplyHiddenState()
        {
            _target.localRotation = Quaternion.identity;
        }
    }
}