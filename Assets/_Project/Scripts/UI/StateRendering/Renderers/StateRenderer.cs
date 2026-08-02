using System;
using System.Collections.Generic;
using LL.UI.StateRendering.Effects;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LL.UI.StateRendering.Renderers
{
    internal abstract class StateRenderer : MonoBehaviour
    {
        [ListDrawerSettings(
            DefaultExpandedState = true,
            ListElementLabelName = nameof(StateEffect.DisplayName),
            ShowItemCount = false)]
        [SerializeReference] private List<StateEffect> _effects = new();

        // Used by state effects to synchronize enum states in the Unity Inspector.
        internal abstract Type StateType { get; }

        private int _state;
        private bool _isStarted;

        private void Start()
        {
            _effects ??= new List<StateEffect>();

            foreach (var effect in _effects)
                effect?.Initialize(this);

            _isStarted = true;
            ApplyState(_state, true);
        }

        private void OnEnable()
        {
            if (_isStarted)
                ApplyState(_state, true);
        }

        private void OnDisable()
        {
            if (_isStarted is false)
                return;

            foreach (var effect in _effects)
                effect?.Restore();
        }

        protected void RenderState<TState>(TState state)
            where TState : struct, Enum
        {
            RenderState(Convert.ToInt32(state));
        }

        private void ApplyState(int state, bool instantly)
        {
            foreach (var effect in _effects)
                effect?.Apply(state, instantly);
        }

        private void RenderState(int state)
        {
            if (_state == state)
                return;

            _state = state;

            if (_isStarted && isActiveAndEnabled)
                ApplyState(_state, false);
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            _effects ??= new List<StateEffect>();

            foreach (var effect in _effects)
                effect?.Synchronize(this);
        }

        [OnInspectorGUI]
        private void DrawTestButtons()
        {
            var isPlaying = Application.isPlaying;

            if (isPlaying is false)
            {
                EditorGUILayout.HelpBox(
                    "State testing is available only in Play Mode.",
                    MessageType.Info);
                return;
            }

            GUILayout.BeginHorizontal();

            foreach (var state in Enum.GetValues(StateType))
            {
                var stateValue = Convert.ToInt32(state);
                var stateName = Enum.GetName(StateType, state) ?? stateValue.ToString();

                if (GUILayout.Button(stateName))
                    RenderState(stateValue);
            }

            GUILayout.EndHorizontal();
        }
#endif
    }
}