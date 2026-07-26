using System;
using System.Collections.Generic;
using DG.Tweening;
using LL.UI.VisualStates.Core;
using LL.UI.VisualStates.Effects.Values;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.UI.VisualStates.Effects
{
    [AddComponentMenu("LL/UI/Visual States/Effects/Scale Effect")]
    [HideMonoScript]
    internal sealed class ScaleEffect : TweenStateEffect
    {
        [Required]
        [SerializeField] private RectTransform _target;

        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private List<ScaleStateValue> _states = new();

        private Vector3 _initialScale;

        protected override void CaptureInitialValue()
        {
            if (_target != null)
                _initialScale = _target.localScale;
        }

        protected override void ApplyState(VisualStateId state, bool instantly)
        {
            if (_target == null)
                return;

            var targetScale = TryGetStateValue(_states, state, out var stateValue)
                ? stateValue.Value
                : _initialScale;

            if (ShouldApplyImmediately(instantly))
            {
                StopTransition();
                _target.localScale = targetScale;
                return;
            }

            PlayTransition(_target.DOScale(targetScale, TransitionSeconds));
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
            SynchronizeValues();
        }

        private void SynchronizeValues()
        {
            _states ??= new List<ScaleStateValue>();
            var defaultScale = _target == null ? Vector3.one : _target.localScale;
            SynchronizeStateValues(_states, state => new ScaleStateValue(state, defaultScale));
        }
#endif

        [Serializable]
        private sealed class ScaleStateValue : StateValue<Vector3>
        {
            internal ScaleStateValue(VisualStateSet.StateDefinition state, Vector3 scale) : base(state, scale) { }
        }
    }
}