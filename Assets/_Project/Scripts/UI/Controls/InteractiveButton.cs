using LL.UI.VisualStates.Sources;
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
        [Required]
        [SerializeField] private InteractiveStateSource _stateSource;

        internal Observable<Unit> Clicked => _clicked;

        private readonly Subject<Unit> _clicked = new();

        public void OnPointerClick(PointerEventData _)
        {
            if (CanInteract())
                _clicked.OnNext(Unit.Default);
        }

        public void OnPointerDown(PointerEventData _)
        {
            if (CanInteract())
                _stateSource.Press();
        }

        public void OnPointerUp(PointerEventData _)
        {
            _stateSource.Release();
        }

        public void OnPointerExit(PointerEventData _)
        {
            _stateSource.Release();
        }

        private void OnDisable()
        {
            _stateSource.Release();
        }

        private void OnDestroy()
        {
            _clicked.Dispose();
        }

        internal void SetInteractable(bool isInteractable)
        {
            _stateSource.SetInteractable(isInteractable);
        }

        private bool CanInteract() =>
            isActiveAndEnabled && _stateSource.IsInteractable;
    }
}