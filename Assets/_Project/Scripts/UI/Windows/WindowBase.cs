using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace LL.UI.Windows
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(GraphicRaycaster))]
    internal abstract class WindowBase : MonoBehaviour
    {
        internal abstract Type ParameterType { get; }
        internal abstract IWindowParameters CurrentParameters { get; }

        internal RectTransform RectTransform => (RectTransform)transform;

        internal WindowDefinition Definition { get; private set; }
        internal bool IsVisible { get; private set; }

        internal virtual bool CanClose => true;

        private Canvas _canvas;
        private WindowController _windowController;

        [Inject]
        private void Construct(WindowController windowController)
        {
            _windowController = windowController ?? throw new ArgumentNullException(nameof(windowController));
        }

        protected void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.enabled = false;
        }

        internal void Initialize(WindowDefinition definition)
        {
            if (Definition is not null)
                throw new InvalidOperationException($"{nameof(WindowBase)} is already initialized.");

            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        }

        internal abstract void Show(IWindowParameters parameters);

        internal void Show()
        {
            if (IsVisible)
                return;

            OnShow();

            _canvas.enabled = true;
            IsVisible = true;
        }

        internal void Hide()
        {
            if (IsVisible is false)
                return;

            OnHide();

            _canvas.enabled = false;
            IsVisible = false;
        }

        protected bool TryClose() => _windowController.Back();

        protected virtual void OnShow() { }
        protected virtual void OnHide() { }
    }
}