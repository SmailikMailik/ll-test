using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.UI.StateRendering.Effects
{
    [Serializable]
    internal abstract class TweenStateEffect : StateEffect
    {
        [HorizontalGroup(TransitionGroup, Order = 100), LabelText("Duration"), SuffixLabel("s", true)]
        [MinValue(0f)]
        [SerializeField] private float _transitionSeconds = 0.08f;

        [HorizontalGroup(TransitionGroup, Order = 100), LabelText("Ease")]
        [SerializeField] private Ease _ease = Ease.OutQuad;

        private const string TransitionGroup = "Transition";

        [NonSerialized] private Tween _transition;

        protected float TransitionSeconds => _transitionSeconds;

        protected bool ShouldApplyImmediately(bool immediately) => immediately || _transitionSeconds <= 0f;

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

        protected override void Normalize()
        {
            _transitionSeconds = Mathf.Max(0f, _transitionSeconds);
        }
    }
}