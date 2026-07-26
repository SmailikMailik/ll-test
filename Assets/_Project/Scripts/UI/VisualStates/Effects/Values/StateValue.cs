using System;
using LL.UI.VisualStates.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.UI.VisualStates.Effects.Values
{
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

    [Serializable]
    internal abstract class StateValue<T> : StateValue
    {
        [HideLabel]
        [SerializeField] private T _value;

        internal T Value => _value;

        protected StateValue(VisualStateSet.StateDefinition state, T value) : base(state)
        {
            _value = value;
        }

        protected void SetValue(T value)
        {
            _value = value;
        }
    }
}