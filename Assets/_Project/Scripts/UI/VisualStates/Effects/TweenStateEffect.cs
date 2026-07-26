using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.UI.VisualStates.Effects
{
    internal abstract class TweenStateEffect : VisualStateEffect
    {
        [PropertyOrder(100)]
        [MinValue(0f)]
        [SerializeField] private float _transitionSeconds = 0.08f;

        [PropertyOrder(101)]
        [SerializeField] private Ease _ease = Ease.OutQuad;

        protected float TransitionSeconds => _transitionSeconds;

        private Tween _transition;

        protected bool ShouldApplyImmediately(bool instantly) => instantly || _transitionSeconds <= 0f;

        protected void PlayTransition(Tween transition)
        {
            StopTransition();

            _transition = transition
                .SetEase(_ease)
                .SetUpdate(true)
                .Play();
        }

        protected override void StopTransition()
        {
            _transition?.Kill();
            _transition = null;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            _transitionSeconds = Mathf.Max(0f, _transitionSeconds);
        }
#endif
    }
}