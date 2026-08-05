using LL.UI.StateRendering.Renderers;
using LL.UI.StateRendering.States;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LL.UI.Controls
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
        [SerializeField] private InteractiveStateRenderer _stateRenderer;

        internal Observable<Unit> Clicked => _clicked;

        private readonly Subject<Unit> _clicked = new();

        private InteractiveState _state;

        private bool IsInteractable => _state != InteractiveState.Disabled;
        private bool IsPressed => _state == InteractiveState.Pressed;

        public void OnPointerClick(PointerEventData _)
        {
            if (CanInteract())
                _clicked.OnNext(Unit.Default);
        }

        public void OnPointerDown(PointerEventData _)
        {
            if (CanInteract())
                SetState(InteractiveState.Pressed);
        }

        public void OnPointerUp(PointerEventData _)
        {
            Release();
        }

        public void OnPointerExit(PointerEventData _)
        {
            Release();
        }

        private void OnDisable()
        {
            Release();
        }

        private void OnDestroy()
        {
            _clicked.Dispose();
        }

        internal void SetInteractable(bool isInteractable)
        {
            if (isInteractable == IsInteractable)
                return;

            SetState(isInteractable ? InteractiveState.Normal : InteractiveState.Disabled);
        }

        private bool CanInteract() =>
            isActiveAndEnabled && IsInteractable;

        private void Release()
        {
            if (IsPressed)
                SetState(InteractiveState.Normal);
        }

        private void SetState(InteractiveState state)
        {
            if (_state == state)
                return;

            _state = state;
            _stateRenderer.Render(state);
        }
    }
}