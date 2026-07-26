using System.Collections.Generic;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.UI.VisualStates.Drivers
{
    [RequireComponent(typeof(CommonButton))]
    [AddComponentMenu("LL/UI/Visual States/Drivers/Button")]
    [HideMonoScript]
    internal sealed class ButtonVisualStateDriver : MonoBehaviour
    {
        [Required]
        [SerializeField] private VisualStateController _controller;

        [ValueDropdown(nameof(GetStateOptions))]
        [ValidateInput(nameof(IsValidState), "Select a state from the assigned controller.")]
        [SerializeField] private VisualStateId _normalState;

        [ValueDropdown(nameof(GetStateOptions))]
        [ValidateInput(nameof(IsValidState), "Select a state from the assigned controller.")]
        [SerializeField] private VisualStateId _pressedState;

        [ValueDropdown(nameof(GetStateOptions))]
        [ValidateInput(nameof(IsValidState), "Select a state from the assigned controller.")]
        [SerializeField] private VisualStateId _selectedState;

        [ValueDropdown(nameof(GetStateOptions))]
        [ValidateInput(nameof(IsValidState), "Select a state from the assigned controller.")]
        [SerializeField] private VisualStateId _disabledState;

        private CommonButton _button;
        private bool _isPressed;
        private bool _isInteractable;
        private bool _isSelected;
        private bool _isStarted;

        private void Awake()
        {
            _button = GetComponent<CommonButton>();
        }

        private void Start()
        {
            _isPressed = _button.IsPressed;
            _isInteractable = _button.IsInteractable;
            _isSelected = _button.IsSelected;
            _isStarted = true;
            ApplyState(true);

            _button.Pressed.Subscribe(OnPressedChanged).AddTo(this);
            _button.Interactable.Subscribe(OnInteractableChanged).AddTo(this);
            _button.Selected.Subscribe(OnSelectedChanged).AddTo(this);
        }

        private void OnEnable()
        {
            if (_isStarted)
                ApplyState(true);
        }

        private void OnPressedChanged(bool isPressed)
        {
            if (_isPressed == isPressed)
                return;

            _isPressed = isPressed;
            ApplyStateIfActive();
        }

        private void OnInteractableChanged(bool isInteractable)
        {
            if (_isInteractable == isInteractable)
                return;

            _isInteractable = isInteractable;
            ApplyStateIfActive();
        }

        private void OnSelectedChanged(bool isSelected)
        {
            if (_isSelected == isSelected)
                return;

            _isSelected = isSelected;
            ApplyStateIfActive();
        }

        private void ApplyStateIfActive()
        {
            if (isActiveAndEnabled)
                ApplyState(false);
        }

        private void ApplyState(bool instantly)
        {
            if (_controller == null)
                return;

            var state = _isInteractable is false
                ? _disabledState
                : _isPressed
                    ? _pressedState
                    : _isSelected
                        ? _selectedState
                        : _normalState;

            _controller.SetState(state, instantly);
        }

        private IEnumerable<ValueDropdownItem<VisualStateId>> GetStateOptions()
        {
            if (_controller == null || _controller.StateSet == null)
                yield break;

            foreach (var state in _controller.StateSet.States)
            {
                if (state != null)
                    yield return new ValueDropdownItem<VisualStateId>(state.Name, state.Id);
            }
        }

        private bool IsValidState(VisualStateId state) =>
            _controller == null ||
            _controller.StateSet == null ||
            _controller.StateSet.Contains(state);

#if UNITY_EDITOR
        private void Reset()
        {
            var controllers = GetComponents<VisualStateController>();

            if (controllers.Length == 1)
                _controller = controllers[0];
        }
#endif
    }
}