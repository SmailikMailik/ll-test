using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.UI.VisualStates.Sources
{
    internal enum InteractiveState : byte
    {
        Normal = 0,
        Pressed = 1,
        Disabled = 2
    }

    [DisallowMultipleComponent]
    [AddComponentMenu("LL/UI/Visual States/Sources/Interactive State Source")]
    [HideMonoScript]
    internal sealed class InteractiveStateSource : VisualStateSource
    {
        internal bool IsInteractable => Value != InteractiveState.Disabled;
        internal bool IsPressed => Value == InteractiveState.Pressed;
        internal override Type StateType => typeof(InteractiveState);

        private InteractiveState Value => (InteractiveState)State.Value;

        internal void Press()
        {
            if (IsInteractable)
                SetState(InteractiveState.Pressed);
        }

        internal void Release()
        {
            if (IsPressed)
                SetState(InteractiveState.Normal);
        }

        internal void SetInteractable(bool isInteractable)
        {
            if (isInteractable == IsInteractable)
                return;

            SetState(isInteractable ? InteractiveState.Normal : InteractiveState.Disabled);
        }
    }
}