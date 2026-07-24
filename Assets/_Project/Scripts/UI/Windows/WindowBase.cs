using UnityEngine;
using VContainer;

namespace LL.UI.Windows
{
    internal interface IWindowParameters { }

    internal abstract class Window<TParameter> : WindowBase
        where TParameter : class, IWindowParameters
    {
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
        internal WindowType Type { get; private set; }
        internal WindowData Data { get; private set; }
        internal bool IsVisible { get; private set; }

        protected virtual bool CanGoBack => true;

        private Canvas _canvas;
        private WindowsController _windowsController;

        [Inject]
        private void Construct(WindowsController windowsController)
        {
            _windowsController = windowsController;
        }

        protected void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.enabled = false;
        }

        internal void Initialize(WindowType type, WindowData data)
        {
            Type = type;
            Data = data;
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
                _windowsController.Back();
        }

        protected virtual void OnShow() { }
        protected virtual void OnHide() { }
    }
}