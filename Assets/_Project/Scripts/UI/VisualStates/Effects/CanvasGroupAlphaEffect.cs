using System;
using System.Collections.Generic;
using DG.Tweening;
using LL.UI.VisualStates.Effects.Values;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.UI.VisualStates.Effects
{
    [Serializable]
    internal sealed class CanvasGroupAlphaEffect : TweenStateEffect
    {
        [SerializeField, Required] private CanvasGroup _target;

        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private List<AlphaStateValue> _states = new();

        [NonSerialized] private float _initialAlpha;

        protected override void CaptureInitialValue()
        {
            if (_target != null)
                _initialAlpha = _target.alpha;
        }

        protected override void ApplyState(int state, bool instantly)
        {
            if (_target == null)
                return;

            var targetAlpha = TryGetStateValue(_states, state, out var stateValue)
                ? stateValue.Value
                : _initialAlpha;

            if (ShouldApplyImmediately(instantly))
            {
                StopTransition();
                _target.alpha = targetAlpha;
                return;
            }

            PlayTransition(_target.DOFade(targetAlpha, TransitionSeconds));
        }

        protected override void RestoreInitialValue()
        {
            if (_target != null)
                _target.alpha = _initialAlpha;
        }

        protected override void SynchronizeValues(Type stateType)
        {
            _states ??= new List<AlphaStateValue>();
            var defaultAlpha = _target == null ? 1f : _target.alpha;
            SynchronizeStateValues(
                _states,
                stateType,
                (state, stateName) => new AlphaStateValue(state, stateName, defaultAlpha));

            foreach (var state in _states)
                state.Clamp();
        }
    }
}