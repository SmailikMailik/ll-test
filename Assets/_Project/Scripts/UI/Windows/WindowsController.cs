using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace LL.UI.Windows
{
    internal sealed class WindowsController : MonoBehaviour
    {
        [SerializeField] private Transform _container;

        internal ReactiveParameter<WindowBase> CurrentWindow { get; } = new();
        internal bool CanGoBack => _history.Count > 1 || CurrentWindow.Value?.Data.IsPopup == true;

        private readonly Dictionary<WindowType, List<WindowBase>> _createdWindows = new();
        private readonly Stack<WindowBase> _history = new();

        private WindowsSettings _settings;
        private WindowsFactory _factory;

        private void Awake()
        {
            if (_container == null)
            {
                Debug.LogError(
                    "[WindowsController::Awake] Window container is not assigned",
                    this);
            }
        }

        [Inject]
        private void Construct(WindowsSettings settings, WindowsFactory factory)
        {
            _settings = settings;
            _factory = factory;
        }

        internal void Show<TParameter>(WindowType windowType, TParameter parameters)
            where TParameter : class, IWindowParameters
        {
            if (windowType == WindowType.Unknown)
            {
                Debug.LogError(
                    "[WindowsController::Show] Window type is Unknown",
                    this);
                return;
            }

            if (parameters is null)
            {
                Debug.LogError(
                    $"[WindowsController::Show] Parameters for {windowType} are null",
                    this);
                return;
            }

            if (CurrentWindow.Value && CurrentWindow.Value.Type == windowType)
                return;

            if (TryGetWindowData<TParameter>(windowType, out var windowData) is false)
                return;

            var window = GetWindow(windowType, windowData);
            if (window is not Window<TParameter> parameterizedWindow)
            {
                Debug.LogError(
                    $"[WindowsController::Show] Window {windowType} must inherit " +
                    $"{typeof(Window<TParameter>).Name}",
                    this);
                return;
            }

            if (parameterizedWindow.Data.IsPopup is false && CurrentWindow.Value != null)
                CurrentWindow.Value.Hide();

            parameterizedWindow.RectTransform.SetAsLastSibling();
            _history.Push(parameterizedWindow);

            parameterizedWindow.Show(parameters);
            CurrentWindow.Value = parameterizedWindow;
        }

        internal void Back()
        {
            if (CanGoBack is false)
                return;

            var currentWindow = _history.Pop();

            currentWindow.Hide();

            if (_history.Count == 0)
            {
                CurrentWindow.Value = null;
                return;
            }

            var previousWindow = _history.Peek();

            if (previousWindow.IsVisible is false)
                previousWindow.Show();

            CurrentWindow.Value = previousWindow;
        }

        private bool TryGetWindowData<TParameter>(WindowType windowType, out WindowData windowData)
            where TParameter : class, IWindowParameters
        {
            if (_settings.Items.TryGetValue(windowType, out windowData) is false)
            {
                Debug.LogError(
                    $"[WindowsController::Show] Window {windowType} is not configured",
                    this);
                return false;
            }

            if (windowData.Prefab == null)
            {
                Debug.LogError(
                    $"[WindowsController::Show] Prefab for {windowType} is not assigned",
                    this);
                return false;
            }

            if (windowData.Prefab is not Window<TParameter>)
            {
                Debug.LogError(
                    $"[WindowsController::Show] Window {windowType} does not accept " +
                    $"{typeof(TParameter).Name}",
                    this);
                return false;
            }

            return true;
        }

        private WindowBase GetWindow(WindowType windowType, WindowData windowData)
        {
            return GetReusableWindow(windowType) ?? CreateWindow(windowType, windowData);
        }

        private WindowBase GetReusableWindow(WindowType windowType)
        {
            if (_createdWindows.TryGetValue(windowType, out var windows) is false)
                return null;

            for (var i = windows.Count - 1; i >= 0; i--)
            {
                var window = windows[i];

                if (window == null)
                {
                    windows.RemoveAt(i);
                    continue;
                }

                if (window.IsVisible is false && _history.Contains(window) is false)
                    return window;
            }

            return null;
        }

        private WindowBase CreateWindow(WindowType windowType, WindowData windowData)
        {
            var window = _factory.Create(windowData.Prefab, _container);

            if (_createdWindows.TryGetValue(windowType, out var windows))
                windows.Add(window);
            else
                _createdWindows.Add(windowType, new List<WindowBase> { window });

            window.name = $"{windowType}";
            window.Initialize(windowType, windowData);

            return window;
        }
    }
}