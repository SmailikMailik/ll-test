using System;
using UnityEngine;
using VContainer;

namespace LL.UI.Windows
{
    internal interface IWindowParameters { }

    internal abstract class Window<TParameter> : WindowBase
        where TParameter : class, IWindowParameters
    {
        internal override Type ParameterType => typeof(TParameter);

        protected TParameter Parameters { get; private set; }

        internal void Show(TParameter parameters)
        {
            Parameters = parameters;
            Show();
        }
    }

    [RequireComponent(typeof(Canvas))]
    internal abstract class WindowBase : RectMonoBehaviour
    {
        internal abstract Type ParameterType { get; }

        internal WindowDefinition Definition { get; private set; }
        internal bool IsVisible { get; private set; }

        protected virtual bool CanGoBack => true;

        private Canvas _canvas;
        private WindowController _windowController;

        [Inject]
        private void Construct(WindowController windowController)
        {
            _windowController = windowController;
        }

        protected void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.enabled = false;
        }

        internal void Initialize(WindowDefinition definition)
        {
            Definition = definition;
        }

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

        internal void Back()
        {
            if (CanGoBack)
                _windowController.Back();
        }

        protected virtual void OnShow() { }
        protected virtual void OnHide() { }
    }
}