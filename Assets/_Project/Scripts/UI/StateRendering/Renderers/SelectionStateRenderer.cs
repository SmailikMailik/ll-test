using System;
using LL.UI.StateRendering.States;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.UI.StateRendering.Renderers
{
    [DisallowMultipleComponent]
    [AddComponentMenu("LL/UI/State Rendering/Selection State Renderer")]
    [HideMonoScript]
    internal sealed class SelectionStateRenderer : StateRenderer
    {
        internal override Type StateType => typeof(SelectionState);

        internal void Render(SelectionState state)
        {
            RenderState(state);
        }
    }
}