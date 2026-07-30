using System;
using System.Collections.Generic;
using LL.UI.VisualStates.Effects.Values;
using LL.UI.VisualStates.Sources;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.UI.VisualStates.Effects
{
    internal abstract class VisualStateEffect : MonoBehaviour
    {
        [SerializeField, Required] private VisualStateSource _source;

        private bool _isStarted;

        private void Start()
        {
            if (_source == null)
            {
                Debug.LogError(
                    $"{GetType().Name} requires a {nameof(VisualStateSource)}.",
                    this);
                enabled = false;
                return;
            }

            CaptureInitialValue();
            _isStarted = true;

            var applyInstantly = true;

            _source.State
                .Subscribe(state =>
                {
                    if (isActiveAndEnabled)
                        ApplyState(state, applyInstantly);

                    applyInstantly = false;
                })
                .AddTo(this);
        }

        private void OnEnable()
        {
            if (_isStarted)
                ApplyState(_source.State.Value, true);
        }

        private void OnDisable()
        {
            if (_isStarted is false)
                return;

            StopTransition();
            RestoreInitialValue();
        }

        protected abstract void CaptureInitialValue();
        protected abstract void ApplyState(int state, bool instantly);
        protected virtual void StopTransition() { }
        protected abstract void RestoreInitialValue();

        protected bool TryGetStateValue<TValue>(
            IReadOnlyList<TValue> values,
            int state,
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
            TryAssignSource();
        }

        protected virtual void OnValidate()
        {
            TryAssignSource();
        }

        protected void SynchronizeStateValues<TValue>(
            List<TValue> values,
            Func<int, string, TValue> createValue)
            where TValue : StateValue
        {
            if (_source == null || values == null)
                return;

            var stateType = _source.StateType;

            if (stateType == null || stateType.IsEnum is false)
                return;

            var existingValues = new Dictionary<int, TValue>();

            foreach (var value in values)
            {
                if (value != null)
                    existingValues.TryAdd(value.State, value);
            }

            values.Clear();

            foreach (var value in Enum.GetValues(stateType))
            {
                var state = Convert.ToInt32(value);
                var stateName = Enum.GetName(stateType, value) ?? state.ToString();

                if (existingValues.TryGetValue(state, out var stateValue) is false)
                    stateValue = createValue(state, stateName);

                stateValue.UpdateName(stateName);
                values.Add(stateValue);
            }
        }

        private void TryAssignSource()
        {
            if (_source != null)
                return;

            var sources = GetComponentsInParent<VisualStateSource>(true);

            if (sources.Length == 1)
                _source = sources[0];
        }
#endif
    }
}