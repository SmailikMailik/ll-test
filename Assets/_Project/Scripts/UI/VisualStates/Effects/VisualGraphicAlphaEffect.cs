using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace LL.UI.VisualStates.Effects
{
    [AddComponentMenu("LL/UI/Visual States/Effects/Graphic Alpha")]
    [HideMonoScript]
    internal sealed class VisualGraphicAlphaEffect : VisualStateEffect
    {
        [Required]
        [SerializeField] private Graphic _target;

        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private List<AlphaStateValue> _states = new();

        [MinValue(0f)]
        [SerializeField] private float _transitionSeconds = 0.08f;

        [SerializeField] private Ease _ease = Ease.OutQuad;

        private float _initialAlpha;
        private Tween _transition;

        protected override void CaptureInitialValue()
        {
            if (_target != null)
                _initialAlpha = _target.color.a;
        }

        protected override void ApplyState(VisualStateId state, bool instantly)
        {
            if (_target == null)
                return;

            StopTransition();

            var targetAlpha = TryGetStateValue(_states, state, out var stateValue)
                ? stateValue.Alpha
                : _initialAlpha;

            if (instantly || _transitionSeconds <= 0f)
            {
                SetAlpha(targetAlpha);
                return;
            }

            _transition = _target
                .DOFade(targetAlpha, _transitionSeconds)
                .SetEase(_ease)
                .SetUpdate(true);
        }

        protected override void StopTransition()
        {
            _transition?.Kill();
            _transition = null;
        }

        protected override void RestoreInitialValue()
        {
            if (_target != null)
                SetAlpha(_initialAlpha);
        }

        private void SetAlpha(float alpha)
        {
            var color = _target.color;
            color.a = alpha;
            _target.color = color;
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            _target = GetComponent<Graphic>();
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
            _states ??= new List<AlphaStateValue>();
            var defaultAlpha = _target == null ? 1f : _target.color.a;
            SynchronizeStateValues(
                _states,
                state => new AlphaStateValue(state, defaultAlpha));
        }
#endif

        [Serializable]
        private sealed class AlphaStateValue : StateValue
        {
            [HideLabel]
            [Range(0f, 1f)]
            [SerializeField] private float _alpha;

            internal float Alpha => _alpha;

            internal AlphaStateValue(
                VisualStateSet.StateDefinition state,
                float alpha)
                : base(state)
            {
                _alpha = alpha;
            }
        }
    }
}