using System;
using System.Collections.Generic;
using LL.UI.VisualStates.Effects.Values;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.UI.VisualStates.Effects
{
    [AddComponentMenu("LL/UI/Visual States/Effects/Object Toggle Effect")]
    [HideMonoScript]
    internal sealed class ObjectToggleEffect : VisualStateEffect
    {
        [ValidateInput(nameof(IsValidTarget), "Target cannot be this GameObject or one of its parents.")]
        [SerializeField, Required] private GameObject _target;

        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private List<ActiveStateValue> _states = new();

        private bool _initialActive;

        protected override void CaptureInitialValue()
        {
            if (_target != null)
                _initialActive = _target.activeSelf;
        }

        protected override void ApplyState(int state, bool instantly)
        {
            if (_target == null || IsValidTarget(_target) is false)
                return;

            var active = TryGetStateValue(_states, state, out var stateValue)
                ? stateValue.Value
                : _initialActive;

            _target.SetActive(active);
        }

        protected override void RestoreInitialValue()
        {
            if (_target != null && IsValidTarget(_target))
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
                (state, stateName) => new ActiveStateValue(state, stateName, defaultActive));
        }
#endif

        private bool IsValidTarget(GameObject target) =>
            target == null ||
            target != gameObject && transform.IsChildOf(target.transform) is false;

        [Serializable]
        private sealed class ActiveStateValue : StateValue<bool>
        {
            internal ActiveStateValue(
                int state,
                string stateName,
                bool active) :
                base(state, stateName, active) { }
        }
    }
}