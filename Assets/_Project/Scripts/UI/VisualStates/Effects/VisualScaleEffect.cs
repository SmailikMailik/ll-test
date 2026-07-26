using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.UI.VisualStates.Effects
{
    [AddComponentMenu("LL/UI/Visual States/Effects/Scale Effect")]
    [HideMonoScript]
    internal sealed class VisualScaleEffect : VisualStateEffect
    {
        [Required]
        [SerializeField] private RectTransform _target;

        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private List<ScaleStateValue> _states = new();

        [MinValue(0f)]
        [SerializeField] private float _transitionSeconds = 0.08f;

        [SerializeField] private Ease _ease = Ease.OutQuad;

        private Vector3 _initialScale;
        private Tween _transition;

        protected override void CaptureInitialValue()
        {
            if (_target != null)
                _initialScale = _target.localScale;
        }

        protected override void ApplyState(VisualStateId state, bool instantly)
        {
            if (_target == null)
                return;

            StopTransition();

            var targetScale = TryGetStateValue(_states, state, out var stateValue)
                ? stateValue.Scale
                : _initialScale;

            if (instantly || _transitionSeconds <= 0f)
            {
                _target.localScale = targetScale;
                return;
            }

            _transition = _target
                .DOScale(targetScale, _transitionSeconds)
                .SetEase(_ease)
                .SetUpdate(true)
                .Play();
        }

        protected override void StopTransition()
        {
            _transition?.Kill();
            _transition = null;
        }

        protected override void RestoreInitialValue()
        {
            if (_target != null)
                _target.localScale = _initialScale;
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            _target = (RectTransform)transform;
            SynchronizeValues();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            _transitionSeconds = Mathf.Max(0f, _transitionSeconds);
            SynchronizeValues();
        }

        private void SynchronizeValues()
        {
            _states ??= new List<ScaleStateValue>();
            var defaultScale = _target == null ? Vector3.one : _target.localScale;
            SynchronizeStateValues(
                _states,
                state => new ScaleStateValue(state, defaultScale));
        }
#endif

        [Serializable]
        private sealed class ScaleStateValue : StateValue
        {
            [HideLabel]
            [SerializeField] private Vector3 _scale;

            internal Vector3 Scale => _scale;

            internal ScaleStateValue(
                VisualStateSet.StateDefinition state,
                Vector3 scale)
                : base(state)
            {
                _scale = scale;
            }
        }
    }
}