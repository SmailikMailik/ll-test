using LL.UI.Controls.Buttons;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.UI.VisualStates.Drivers
{
    [RequireComponent(typeof(InteractiveButton))]
    [AddComponentMenu("LL/UI/Visual States/Drivers/Button Interaction Driver")]
    [HideMonoScript]
    internal sealed class ButtonInteractionStateDriver : MonoBehaviour
    {
        [Required]
        [SerializeField] private VisualStateController _controller;

        [ValidateInput(nameof(IsValidState), "Choose a state from the assigned controller.")]
        [SerializeField] private VisualStateId _normalState;

        [ValidateInput(nameof(IsValidState), "Choose a state from the assigned controller.")]
        [SerializeField] private VisualStateId _pressedState;

        [ValidateInput(nameof(IsValidState), "Choose a state from the assigned controller.")]
        [SerializeField] private VisualStateId _disabledState;

        private InteractiveButton _button;

        private bool _isPressed;
        private bool _isInteractable;
        private bool _isStarted;

        private void Awake()
        {
            _button = GetComponent<InteractiveButton>();
        }

        private void Start()
        {
            _isPressed = _button.IsPressed;
            _isInteractable = _button.IsInteractable;
            _isStarted = true;

            ApplyState(true);

            _button.Pressed.Subscribe(OnPressedChanged).AddTo(this);
            _button.Interactable.Subscribe(OnInteractableChanged).AddTo(this);
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

        private void ApplyStateIfActive()
        {
            if (isActiveAndEnabled)
                ApplyState(false);
        }

        private void ApplyState(bool instantly)
        {
            if (_controller == null)
                return;

            _controller.SetState(ResolveState(), instantly);
        }

        private VisualStateId ResolveState()
        {
            if (_isInteractable is false)
                return _disabledState;

            if (_isPressed)
                return _pressedState;

            return _normalState;
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