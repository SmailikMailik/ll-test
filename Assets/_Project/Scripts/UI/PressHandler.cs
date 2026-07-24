using R3;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LL.UI
{
    internal sealed class PressHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        internal Observable<bool> Pressed => _pressed;

        private readonly ReactiveProperty<bool> _pressed = new();

        public void OnPointerDown(PointerEventData eventData) => _pressed.Value = true;
        public void OnPointerUp(PointerEventData eventData) => _pressed.Value = false;
        public void OnPointerExit(PointerEventData eventData) => _pressed.Value = false;

        private void OnDisable() => _pressed.Value = false;
        private void OnDestroy() => _pressed.Dispose();
    }
}