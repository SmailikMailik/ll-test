using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.UI.StateRendering.Effects
{
    [Serializable]
    internal abstract class TweenStateEffect : StateEffect
    {
        [HorizontalGroup("Transition", Order = 100), LabelText("Duration"), SuffixLabel("s", true)]
        [MinValue(0f)]
        [SerializeField] private float _transitionSeconds = 0.08f;

        [HorizontalGroup("Transition", Order = 100), LabelText("Ease")]
        [SerializeField] private Ease _ease = Ease.OutQuad;

        protected float TransitionSeconds => _transitionSeconds;

        [NonSerialized] private Tween _transition;

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

        protected override void Normalize()
        {
            _transitionSeconds = Mathf.Max(0f, _transitionSeconds);
        }
    }
}