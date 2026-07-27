using System;
using R3;
using UnityEngine;

namespace LL.UI.VisualStates.Sources
{
    internal abstract class VisualStateSource : MonoBehaviour
    {
        // Used by visual effects to synchronize enum states in the Unity Inspector.
        internal abstract Type StateType { get; }
        internal ReactiveProperty<int> State { get; } = new();

        protected void SetState<TState>(TState state)
            where TState : struct, Enum
        {
            State.Value = Convert.ToInt32(state);
        }

        protected virtual void OnDestroy()
        {
            State.Dispose();
        }
    }
}