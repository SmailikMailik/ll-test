using System;
using UnityEngine;

namespace LL.UI.StateRendering.Effects.Values
{
    [Serializable]
    internal sealed class AlphaStateValue : StateValue<float>
    {
        internal AlphaStateValue(int state, string stateName, float alpha) :
            base(state, stateName, alpha) { }

        internal void Clamp()
        {
            SetValue(Mathf.Clamp01(Value));
        }
    }
}