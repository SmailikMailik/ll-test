using System;
using System.Collections.Generic;
using LL.UI.VisualStates.Effects;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LL.UI.VisualStates.Sources
{
    internal abstract class VisualStateSource : MonoBehaviour
    {
        [SerializeReference] private List<VisualStateEffect> _effects = new();

        // Used by visual effects to synchronize enum states in the Unity Inspector.
        internal abstract Type StateType { get; }
        internal ReactiveProperty<int> State { get; } = new();

        private bool _isStarted;

        private void Start()
        {
            _effects ??= new List<VisualStateEffect>();

            foreach (var effect in _effects)
                effect?.Initialize(this);

            _isStarted = true;
            var applyInstantly = true;

            State
                .Subscribe(state =>
                {
                    if (isActiveAndEnabled)
                        ApplyState(state, applyInstantly);

                    applyInstantly = false;
                })
                .AddTo(this);
        }

        private void OnEnable()
        {
            if (_isStarted)
                ApplyState(State.Value, true);
        }

        private void OnDisable()
        {
            if (_isStarted is false)
                return;

            foreach (var effect in _effects)
                effect?.Restore();
        }

        protected void SetState<TState>(TState state)
            where TState : struct, Enum
        {
            State.Value = Convert.ToInt32(state);
        }

        protected virtual void OnDestroy()
        {
            State.Dispose();
        }

        private void ApplyState(int state, bool instantly)
        {
            foreach (var effect in _effects)
                effect?.Apply(state, instantly);
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            _effects ??= new List<VisualStateEffect>();

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