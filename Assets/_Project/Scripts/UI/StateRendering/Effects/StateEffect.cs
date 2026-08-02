using System;
using System.Collections.Generic;
using LL.UI.StateRendering.Effects.Values;
using LL.UI.StateRendering.Renderers;

namespace LL.UI.StateRendering.Effects
{
    [Serializable]
    internal abstract class StateEffect
    {
        [NonSerialized] private StateRenderer _renderer;

        protected StateRenderer Renderer => _renderer;

        internal void Initialize(StateRenderer renderer)
        {
            _renderer = renderer;
            CaptureInitialValue();
        }

        internal void Apply(int state, bool instantly)
        {
            ApplyState(state, instantly);
        }

        internal void Restore()
        {
            StopTransition();
            RestoreInitialValue();
        }

        internal void Synchronize(StateRenderer renderer)
        {
            _renderer = renderer;
            Normalize();
            SynchronizeValues(renderer.StateType);
        }

        protected abstract void CaptureInitialValue();
        protected abstract void ApplyState(int state, bool instantly);
        protected virtual void StopTransition() { }
        protected abstract void RestoreInitialValue();
        protected virtual void Normalize() { }

        protected bool TryGetStateValue<TValue>(
            IReadOnlyList<TValue> values,
            int state,
            out TValue stateValue)
            where TValue : StateValue
        {
            if (values is not null)
            {
                foreach (var value in values)
                {
                    if (value is not null && value.State == state)
                    {
                        stateValue = value;
                        return true;
                    }
                }
            }

            stateValue = null;
            return false;
        }

        protected virtual void SynchronizeValues(Type stateType) { }

        protected void SynchronizeStateValues<TValue>(
            List<TValue> values,
            Type stateType,
            Func<int, string, TValue> createValue)
            where TValue : StateValue
        {
            if (values is null || stateType is null || stateType.IsEnum is false)
                return;

            var existingValues = new Dictionary<int, TValue>();

            foreach (var value in values)
            {
                if (value is not null)
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
    }
}