using System;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.UI.VisualStates
{
    [AddComponentMenu("LL/UI/Visual States/State Controller")]
    [HideMonoScript]
    internal sealed class VisualStateController : MonoBehaviour
    {
        [Required]
        [SerializeField] private VisualStateSet _stateSet;
        [SerializeField] private VisualStateId _initialState;

        internal VisualStateSet StateSet => _stateSet;
        internal VisualStateId CurrentState { get; private set; }
        internal Observable<StateChange> StateChanged => _stateChanged;

        private readonly Subject<StateChange> _stateChanged = new();
        private bool _isInitialized;

        [ShowInInspector]
        [ReadOnly]
        [ShowIf(nameof(IsPlaying))]
        [LabelText("Current State")]
        private string CurrentStateName => _stateSet == null ? string.Empty : _stateSet.GetName(CurrentState);

        private bool IsPlaying => Application.isPlaying;

        private void Awake()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            _stateChanged.Dispose();
        }

        internal void SetState(VisualStateId state, bool instantly = false)
        {
            Initialize();

            if (_stateSet == null || _stateSet.Contains(state) is false)
            {
                Debug.LogError(
                    $"State '{state}' does not belong to the assigned state set.",
                    this);
                return;
            }

            if (CurrentState == state)
                return;

            var previousState = CurrentState;
            CurrentState = state;
            _stateChanged.OnNext(new StateChange(previousState, state, instantly));
        }

        private void Initialize()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;

            if (_stateSet == null)
                return;

            if (_stateSet.DefaultState.IsEmpty)
            {
                Debug.LogError("The assigned state set does not contain any states.", this);
                return;
            }

            CurrentState = _stateSet.Contains(_initialState)
                ? _initialState
                : _stateSet.DefaultState;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_stateSet == null)
            {
                _initialState = default;
                return;
            }

            if (_stateSet.Contains(_initialState) is false)
                _initialState = _stateSet.DefaultState;
        }
#endif

        internal readonly struct StateChange
        {
            internal VisualStateId PreviousState { get; }
            internal VisualStateId CurrentState { get; }
            internal bool Instantly { get; }

            internal StateChange(
                VisualStateId previousState,
                VisualStateId currentState,
                bool instantly)
            {
                PreviousState = previousState;
                CurrentState = currentState;
                Instantly = instantly;
            }
        }
    }
}