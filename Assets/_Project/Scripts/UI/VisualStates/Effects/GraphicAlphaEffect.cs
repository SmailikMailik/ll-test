using System.Collections.Generic;
using DG.Tweening;
using LL.Extensions;
using LL.UI.VisualStates.Effects.Values;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace LL.UI.VisualStates.Effects
{
    [AddComponentMenu("LL/UI/Visual States/Effects/Graphic Alpha Effect")]
    [HideMonoScript]
    internal sealed class GraphicAlphaEffect : TweenStateEffect
    {
        [Required]
        [SerializeField] private Graphic _target;

        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private List<AlphaStateValue> _states = new();

        private float _initialAlpha;

        protected override void CaptureInitialValue()
        {
            if (_target != null)
                _initialAlpha = _target.color.a;
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
                _target.SetAlpha(targetAlpha);
                return;
            }

            PlayTransition(_target.DOFade(targetAlpha, TransitionSeconds));
        }

        protected override void RestoreInitialValue()
        {
            if (_target != null)
                _target.SetAlpha(_initialAlpha);
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
            SynchronizeValues();
        }

        private void SynchronizeValues()
        {
            _states ??= new List<AlphaStateValue>();
            var defaultAlpha = _target == null ? 1f : _target.color.a;
            SynchronizeStateValues(
                _states,
                (state, stateName) => new AlphaStateValue(state, stateName, defaultAlpha));

            foreach (var state in _states)
                state.Clamp();
        }
#endif

    }
}