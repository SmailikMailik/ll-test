using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.UI.VisualStates.Sources
{
    internal enum SelectionState
    {
        Normal = 0,
        Selected = 1
    }

    [DisallowMultipleComponent]
    [AddComponentMenu("LL/UI/Visual States/Sources/Selection State Source")]
    [HideMonoScript]
    internal sealed class SelectionStateSource : VisualStateSource
    {
        internal bool IsSelected => Value == SelectionState.Selected;
        internal override Type StateType => typeof(SelectionState);

        private SelectionState Value => (SelectionState)State.Value;

        internal void SetSelected(bool isSelected)
        {
            SetState(isSelected ? SelectionState.Selected : SelectionState.Normal);
        }
    }
}