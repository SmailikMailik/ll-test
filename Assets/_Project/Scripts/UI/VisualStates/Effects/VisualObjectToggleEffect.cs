using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.UI.VisualStates.Effects
{
    [AddComponentMenu("LL/UI/Visual States/Effects/Object Toggle")]
    [HideMonoScript]
    internal sealed class VisualObjectToggleEffect : VisualStateEffect
    {
        [Required]
        [ValidateInput(nameof(IsValidTarget), "The effect cannot toggle its own GameObject.")]
        [SerializeField] private GameObject _target;

        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private List<ActiveStateValue> _states = new();

        private bool _initialActive;

        protected override void CaptureInitialValue()
        {
            if (_target != null)
                _initialActive = _target.activeSelf;
        }

        protected override void ApplyState(VisualStateId state, bool instantly)
        {
            if (_target == null || _target == gameObject)
                return;

            var active = TryGetStateValue(_states, state, out var stateValue)
                ? stateValue.Active
                : _initialActive;

            _target.SetActive(active);
        }

        protected override void StopTransition() { }

        protected override void RestoreInitialValue()
        {
            if (_target != null && _target != gameObject)
                _target.SetActive(_initialActive);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            SynchronizeValues();
        }

        private void SynchronizeValues()
        {
            _states ??= new List<ActiveStateValue>();
            var defaultActive = _target != null && _target.activeSelf;
            SynchronizeStateValues(
                _states,
                state => new ActiveStateValue(state, defaultActive));
        }
#endif

        private bool IsValidTarget(GameObject target) =>
            target == null ||
            target != gameObject &&
            transform.IsChildOf(target.transform) is false;

        [Serializable]
        private sealed class ActiveStateValue : StateValue
        {
            [HideLabel]
            [SerializeField] private bool _active;

            internal bool Active => _active;

            internal ActiveStateValue(
                VisualStateSet.StateDefinition state,
                bool active)
                : base(state)
            {
                _active = active;
            }
        }
    }
}