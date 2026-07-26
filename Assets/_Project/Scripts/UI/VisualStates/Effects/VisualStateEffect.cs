using System;
using System.Collections.Generic;
using LL.UI.VisualStates.Core;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.UI.VisualStates.Effects
{
    internal abstract class VisualStateEffect : MonoBehaviour
    {
        [Required]
        [SerializeField] private VisualStateController _controller;

        private bool _isStarted;

        private void Start()
        {
            if (_controller == null)
            {
                enabled = false;
                return;
            }

            CaptureInitialValue();
            _isStarted = true;
            ApplyState(_controller.CurrentState, true);
            _controller.StateChanged.Subscribe(OnStateChanged).AddTo(this);
        }

        private void OnEnable()
        {
            if (_isStarted)
                ApplyState(_controller.CurrentState, true);
        }

        private void OnDisable()
        {
            if (_isStarted is false)
                return;

            StopTransition();
            RestoreInitialValue();
        }

        private void OnStateChanged(VisualStateController.StateChange stateChange)
        {
            if (isActiveAndEnabled is false)
                return;

            ApplyState(stateChange.State, stateChange.Instantly);
        }

        protected abstract void CaptureInitialValue();
        protected abstract void ApplyState(VisualStateId state, bool instantly);
        protected virtual void StopTransition() { }
        protected abstract void RestoreInitialValue();

        protected bool TryGetStateValue<TValue>(
            IReadOnlyList<TValue> values,
            VisualStateId state,
            out TValue stateValue)
            where TValue : StateValue
        {
            if (values != null)
            {
                foreach (var value in values)
                {
                    if (value != null && value.State == state)
                    {
                        stateValue = value;
                        return true;
                    }
                }
            }

            stateValue = null;
            return false;
        }

#if UNITY_EDITOR
        protected virtual void Reset()
        {
            TryAssignController();
        }

        protected virtual void OnValidate()
        {
            TryAssignController();
        }

        protected void SynchronizeStateValues<TValue>(
            List<TValue> values,
            Func<VisualStateSet.StateDefinition, TValue> createValue)
            where TValue : StateValue
        {
            if (_controller == null || _controller.StateSet == null || values == null)
                return;

            var existingValues = new Dictionary<VisualStateId, TValue>();

            foreach (var value in values)
            {
                if (value != null)
                    existingValues.TryAdd(value.State, value);
            }

            values.Clear();

            foreach (var state in _controller.StateSet.States)
            {
                if (state == null)
                    continue;

                if (existingValues.TryGetValue(state.Id, out var value) is false)
                    value = createValue(state);

                value.UpdateName(state.Name);
                values.Add(value);
            }
        }

        private void TryAssignController()
        {
            if (_controller != null)
                return;

            var controllers = GetComponentsInParent<VisualStateController>(true);

            if (controllers.Length == 1)
                _controller = controllers[0];
        }
#endif

        [Serializable]
        internal abstract class StateValue
        {
            [HideInInspector]
            [SerializeField] private VisualStateId _state;

            [ReadOnly]
            [TableColumnWidth(160)]
            [SerializeField] private string _stateName;

            internal VisualStateId State => _state;

            protected StateValue(VisualStateSet.StateDefinition state)
            {
                _state = state.Id;
                _stateName = state.Name;
            }

            internal void UpdateName(string stateName)
            {
                _stateName = stateName;
            }
        }
    }
}