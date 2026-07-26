using R3;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LL.UI.Controls.Buttons
{
    [DisallowMultipleComponent]
    [AddComponentMenu("LL/UI/Controls/Interactive Button")]
    [HideMonoScript]
    internal sealed class InteractiveButton :
        MonoBehaviour,
        IPointerClickHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerExitHandler
    {
        internal Observable<Unit> Clicked => _clicked;
        internal Observable<bool> Pressed => _pressed;
        internal Observable<bool> Interactable => _interactable;

        internal bool IsPressed => _pressed.Value;
        internal bool IsInteractable => _interactable.Value;

        private readonly Subject<Unit> _clicked = new();
        private readonly ReactiveProperty<bool> _pressed = new();
        private readonly ReactiveProperty<bool> _interactable = new(true);

        public void OnPointerClick(PointerEventData eventData)
        {
            if (CanInteract(eventData))
                _clicked.OnNext(Unit.Default);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (CanInteract(eventData))
                _pressed.Value = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                _pressed.Value = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _pressed.Value = false;
        }

        private void OnDisable()
        {
            _pressed.Value = false;
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

            if (isInteractable is false)
                _pressed.Value = false;
        }

        private bool CanInteract(PointerEventData eventData) =>
            eventData.button == PointerEventData.InputButton.Left &&
            isActiveAndEnabled &&
            _interactable.Value;
    }
}