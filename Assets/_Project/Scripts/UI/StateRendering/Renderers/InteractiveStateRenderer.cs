using System;
using LL.UI.StateRendering.States;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.UI.StateRendering.Renderers
{
    [DisallowMultipleComponent]
    [AddComponentMenu("LL/UI/State Rendering/Interactive State Renderer")]
    [HideMonoScript]
    internal sealed class InteractiveStateRenderer : StateRenderer
    {
        internal override Type StateType => typeof(InteractiveState);

        internal void Render(InteractiveState state)
        {
            RenderState(state);
        }
    }
}