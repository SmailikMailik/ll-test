using System;
using R3;
using UnityEngine;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEditor;
#endif

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

#if UNITY_EDITOR
        [OnInspectorGUI]
        private void DrawTestButtons()
        {
            var isPlaying = Application.isPlaying;

            if (isPlaying is false)
            {
                EditorGUILayout.HelpBox(
                    "State testing is available only in Play Mode.",
                    MessageType.Info);
            }

            using (new EditorGUI.DisabledScope(isPlaying is false))
            {
                GUILayout.BeginHorizontal();

                foreach (var state in Enum.GetValues(StateType))
                {
                    var stateValue = Convert.ToInt32(state);
                    var stateName = Enum.GetName(StateType, state) ?? stateValue.ToString();

                    if (GUILayout.Button(stateName))
                        State.Value = stateValue;
                }

                GUILayout.EndHorizontal();
            }
        }
#endif
    }
}