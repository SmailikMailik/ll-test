using System;
using System.Collections.Generic;
using DG.Tweening;
using LL.UI.StateRendering.Effects.Values;
using LL.UI.StateRendering.Inspector;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.UI.StateRendering.Effects
{
    [Serializable]
    internal sealed class ScaleEffect : TweenStateEffect
    {
        [SerializeField, Required] private RectTransform _target;

        [StateValuesTable]
        [SerializeField] private List<ScaleStateValue> _states = new();

        [NonSerialized] private Vector3 _initialScale;

        internal override string DisplayName => "Scale";
        internal override bool HasTarget => _target != null;
        internal override IReadOnlyList<StateValue> StateValues => _states;

        protected override void CaptureInitialValue()
        {
            if (IsTargetValid(Renderer))
                _initialScale = _target.localScale;
        }

        protected override void ApplyState(int state, bool immediately)
        {
            if (IsTargetValid(Renderer) is false)
                return;

            var targetScale = TryGetStateValue(_states, state, out var stateValue)
                ? stateValue.Value
                : _initialScale;

            if (ShouldApplyImmediately(immediately))
            {
                StopTransition();
                _target.localScale = targetScale;
                return;
            }

            PlayTransition(_target.DOScale(targetScale, TransitionSeconds));
        }

        protected override void RestoreInitialValue()
        {
            if (IsTargetValid(Renderer))
                _target.localScale = _initialScale;
        }

        protected override void SynchronizeValues(Type stateType)
        {
            _states ??= new List<ScaleStateValue>();
            var defaultScale = IsTargetValid(Renderer) ? _target.localScale : Vector3.one;
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