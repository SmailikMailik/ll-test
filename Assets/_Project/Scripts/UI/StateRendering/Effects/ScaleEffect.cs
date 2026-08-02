using System;
using System.Collections.Generic;
using DG.Tweening;
using LL.UI.StateRendering.Effects.Values;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.UI.StateRendering.Effects
{
    [Serializable]
    internal sealed class ScaleEffect : TweenStateEffect
    {
        [SerializeField, Required] private RectTransform _target;

        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private List<ScaleStateValue> _states = new();

        [NonSerialized] private Vector3 _initialScale;

        protected override void CaptureInitialValue()
        {
            if (_target != null)
                _initialScale = _target.localScale;
        }

        protected override void ApplyState(int state, bool instantly)
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

        protected override void SynchronizeValues(Type stateType)
        {
            _states ??= new List<ScaleStateValue>();
            var defaultScale = _target == null ? Vector3.one : _target.localScale;
            SynchronizeStateValues(
                _states,
                stateType,
                (state, stateName) => new ScaleStateValue(state, stateName, defaultScale));
        }

        [Serializable]
        private sealed class ScaleStateValue : StateValue<Vector3>
        {
            internal ScaleStateValue(
                int state,
                string stateName,
                Vector3 scale) :
                base(state, stateName, scale) { }
        }
    }
}