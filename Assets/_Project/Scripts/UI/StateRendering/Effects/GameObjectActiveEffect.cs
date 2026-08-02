using System;
using System.Collections.Generic;
using LL.UI.StateRendering.Effects.Values;
using LL.UI.StateRendering.Inspector;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.UI.StateRendering.Effects
{
    [Serializable]
    internal sealed class GameObjectActiveEffect : StateEffect
    {
        [ValidateInput(nameof(IsValidTarget), "Target cannot be the renderer GameObject or one of its parents.")]
        [SerializeField, Required] private GameObject _target;

        [StateValuesTable]
        [SerializeField] private List<ActiveStateValue> _states = new();

        [NonSerialized] private bool _initialActive;

        internal override string DisplayName => "Game Object Active";

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
            Renderer == null ||
            target != Renderer.gameObject && Renderer.transform.IsChildOf(target.transform) is false;

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