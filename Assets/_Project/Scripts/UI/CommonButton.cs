using R3;
using UnityEngine;

namespace LL.UI
{
    internal sealed class CommonButton : MonoBehaviour
    {
        [SerializeField] private ClickHandler _clickHandler;
        [SerializeField] private PressHandler _pressHandler;

        internal Observable<Unit> Clicked => _clicked;
        internal Observable<bool> Pressed => _pressed;
        internal Observable<bool> Interactable => _interactable;

        private readonly Subject<Unit> _clicked = new();
        private readonly ReactiveProperty<bool> _pressed = new();
        private readonly ReactiveProperty<bool> _interactable = new(true);

        private void Start()
        {
            _clickHandler.Clicked.Subscribe(OnClicked).AddTo(this);
            _pressHandler.Pressed.Subscribe(OnPressedChanged).AddTo(this);
            _interactable.Subscribe(OnInteractableChanged).AddTo(this);
        }

        private void OnDestroy()
        {
            _clicked.Dispose();
            _pressed.Dispose();
            _interactable.Dispose();
        }

        internal void SetInteractable(bool isInteractable)
        {
            _interactable.Value = isInteractable;
        }

        private void OnInteractableChanged(bool isInteractable)
        {
            if (isInteractable is false)
                _pressed.Value = false;
        }

        private void OnPressedChanged(bool isPressed)
        {
            _pressed.Value = isActiveAndEnabled && _interactable.Value && isPressed;
        }

        private void OnClicked(Unit _)
        {
            if (isActiveAndEnabled && _interactable.Value)
                _clicked.OnNext(Unit.Default);
        }
    }
}