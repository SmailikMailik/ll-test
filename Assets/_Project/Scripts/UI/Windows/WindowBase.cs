using System;
using LL.UI.Windows.Configuration;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace LL.UI.Windows
{
    internal interface IWindowParameters { }

    internal abstract class Window<TParameter> : WindowBase
        where TParameter : class, IWindowParameters
    {
        internal override Type ParameterType => typeof(TParameter);
        internal override IWindowParameters CurrentParameters => Parameters;

        protected TParameter Parameters { get; private set; }

        internal override void Show(IWindowParameters parameters)
        {
            if (parameters is not TParameter typedParameters)
            {
                throw new ArgumentException(
                    $"Expected {typeof(TParameter).Name}, got {parameters?.GetType().Name ?? "null"}",
                    nameof(parameters));
            }

            Parameters = typedParameters;
            Show();
        }
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    internal abstract class WindowBase : RectMonoBehaviour
    {
        internal abstract Type ParameterType { get; }
        internal abstract IWindowParameters CurrentParameters { get; }

        internal WindowDefinition Definition { get; private set; }
        internal bool IsVisible { get; private set; }

        internal virtual bool CanClose => true;

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