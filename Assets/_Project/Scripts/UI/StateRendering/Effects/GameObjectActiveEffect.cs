using System;
using System.Collections.Generic;
using LL.UI.StateRendering.Effects.Values;
using LL.UI.StateRendering.Inspector;
using LL.UI.StateRendering.Renderers;
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
        internal override bool HasTarget => _target != null;
        internal override IReadOnlyList<StateValue> StateValues => _states;

        internal override bool IsTargetValid(StateRenderer renderer) =>
            HasTarget && IsValidTargetForRenderer(_target, renderer);

        protected override void CaptureInitialValue()
        {
            if (IsTargetValid(Renderer))
                _initialActive = _target.activeSelf;
        }

        protected override void ApplyState(int state, bool immediately)
        {
            if (IsTargetValid(Renderer) is false)
                return;

            var active = TryGetStateValue(_states, state, out var stateValue)
                ? stateValue.Value
                : _initialActive;

            _target.SetActive(active);
        }

        protected override void RestoreInitialValue()
        {
            if (IsTargetValid(Renderer))
                _target.SetActive(_initialActive);
        }

        protected override void SynchronizeValues(Type stateType)
        {
            _states ??= new List<ActiveStateValue>();
            var defaultActive = IsTargetValid(Renderer) && _target.activeSelf;
            SynchronizeStateValues(
                _states,
                stateType,
                (state, stateName) => new ActiveStateValue(state, stateName, defaultActive));
        }

        private bool IsValidTarget(GameObject target) =>
            target == null ||
            IsValidTargetForRenderer(target, Renderer);

        private static bool IsValidTargetForRenderer(GameObject target, StateRenderer renderer) =>
            renderer == null ||
            target != renderer.gameObject && renderer.transform.IsChildOf(target.transform) is false;

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