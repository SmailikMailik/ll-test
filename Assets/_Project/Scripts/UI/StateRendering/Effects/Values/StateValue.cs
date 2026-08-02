using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.UI.StateRendering.Effects.Values
{
    [Serializable]
    internal abstract class StateValue
    {
        [HideInInspector]
        [SerializeField] private int _state;

        [ReadOnly]
        [TableColumnWidth(160)]
        [SerializeField] private string _stateName;

        internal int State => _state;

        protected StateValue(int state, string stateName)
        {
            _state = state;
            _stateName = stateName;
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

        protected StateValue(int state, string stateName, T value) : base(state, stateName)
        {
            _value = value;
        }

        protected void SetValue(T value)
        {
            _value = value;
        }
    }
}