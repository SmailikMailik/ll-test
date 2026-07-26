using System;
using LL.UI.VisualStates.Core;
using UnityEngine;

namespace LL.UI.VisualStates.Effects.Values
{
    [Serializable]
    internal sealed class AlphaStateValue : StateValue<float>
    {
        internal AlphaStateValue(VisualStateSet.StateDefinition state, float alpha) : base(state, alpha) { }

        internal void Clamp()
        {
            SetValue(Mathf.Clamp01(Value));
        }
    }
}