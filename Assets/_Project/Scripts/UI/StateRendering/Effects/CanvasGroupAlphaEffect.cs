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
    internal sealed class CanvasGroupAlphaEffect : TweenStateEffect
    {
        [SerializeField, Required] private CanvasGroup _target;

        [StateValuesTable]
        [SerializeField] private List<AlphaStateValue> _states = new();

        [NonSerialized] private float _initialAlpha;

        internal override string DisplayName => "Canvas Group Alpha";
        internal override bool HasTarget => _target != null;
        internal override IReadOnlyList<StateValue> StateValues => _states;

        protected override void CaptureInitialValue()
        {
            if (IsTargetValid(Renderer))
                _initialAlpha = _target.alpha;
        }

        protected override void ApplyState(int state, bool immediately)
        {
            if (IsTargetValid(Renderer) is false)
                return;

            var targetAlpha = TryGetStateValue(_states, state, out var stateValue)
                ? stateValue.Value
                : _initialAlpha;

            if (ShouldApplyImmediately(immediately))
            {
                StopTransition();
                _target.alpha = targetAlpha;
                return;
            }

            PlayTransition(_target.DOFade(targetAlpha, TransitionSeconds));
        }

        protected override void RestoreInitialValue()
        {
            if (IsTargetValid(Renderer))
                _target.alpha = _initialAlpha;
        }

        protected override void SynchronizeValues(Type stateType)
        {
            _states ??= new List<AlphaStateValue>();
            var defaultAlpha = IsTargetValid(Renderer) ? _target.alpha : 1f;
            SynchronizeStateValues(
                _states,
                stateType,
                (state, stateName) => new AlphaStateValue(state, stateName, defaultAlpha));

            foreach (var state in _states)
                state.Clamp();
        }
    }
}