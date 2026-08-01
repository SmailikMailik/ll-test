using System;
using System.Collections.Generic;
using LL.UI.VisualStates.Effects.Values;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace LL.UI.VisualStates.Effects
{
    [Serializable]
    [MovedFrom(true, "LL.UI.VisualStates.Effects", null, "ObjectToggleEffect")]
    internal sealed class GameObjectActiveEffect : VisualStateEffect
    {
        [ValidateInput(nameof(IsValidTarget), "Target cannot be the source GameObject or one of its parents.")]
        [SerializeField, Required] private GameObject _target;

        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private List<ActiveStateValue> _states = new();

        [NonSerialized] private bool _initialActive;

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

        protected override void SynchronizeValues(Type stateType)
        {
            _states ??= new List<ActiveStateValue>();
            var defaultActive = _target != null && _target.activeSelf;
            SynchronizeStateValues(
                _states,
                stateType,
                (state, stateName) => new ActiveStateValue(state, stateName, defaultActive));
        }

        private bool IsValidTarget(GameObject target) =>
            target == null ||
            Source == null ||
            target != Source.gameObject && Source.transform.IsChildOf(target.transform) is false;

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