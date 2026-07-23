using System;
using UnityEngine;

namespace LL.UI
{
    internal sealed class CommonButton : MonoBehaviour
    {
        [SerializeField] private ClickHandler _clickHandler;

        internal event Action Clicked;

        internal ReactiveParameter<bool> IsPressed { get; } = new();
        internal ReactiveParameter<bool> IsInteractable { get; } = new();

        private void OnEnable()
        {
            IsInteractable.Changed += IsInteractableChanged;

            _clickHandler.Clicked += OnClicked;
        }

        private void OnDisable()
        {
            IsInteractable.Changed -= IsInteractableChanged;

            _clickHandler.Clicked -= OnClicked;
        }

        private void IsInteractableChanged(bool isInteractable)
        {
            if (isInteractable is false && IsPressed.Value)
                IsPressed.Value = false;
        }

        private void OnClicked()
        {
            if (IsInteractable.Value is false)
                return;

            Clicked?.Invoke();
        }
    }
}