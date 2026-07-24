using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace LL.UI.Windows
{
    internal sealed class WindowsController : MonoBehaviour
    {
        [SerializeField] private Transform _container;

        internal event Action<WindowType> WindowOpened;
        internal event Action<WindowType> WindowClosed;

        internal WindowBase CurrentWindow { get; private set; }
        internal bool CanGoBack => _history.Count > 1 || (_history.Count == 1 && _history.Peek().Data.IsPopup);

        private readonly Dictionary<WindowType, List<WindowBase>> _createdWindows = new();
        private readonly Stack<WindowBase> _history = new();

        private WindowsSettings _settings;
        private WindowsFactory _factory;

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
                return;

            if (parameters is null)
            {
                Debug.LogError($"[WindowsController::Show] Parameters for {windowType} are null");
                return;
            }

            if (CurrentWindow && CurrentWindow.Type == windowType)
                return;

            if (_settings.Items.TryGetValue(windowType, out var windowData) is false)
            {
                Debug.LogError($"[WindowsController::Show] Window {windowType} is not configured");
                return;
            }

            if (windowData.Prefab == null)
            {
                Debug.LogError($"[WindowsController::Show] Prefab for {windowType} is not assigned");
                return;
            }

            if (windowData.Prefab is not Window<TParameter>)
            {
                Debug.LogError(
                    $"[WindowsController::Show] Window {windowType} does not accept " +
                    $"{typeof(TParameter).Name}");
                return;
            }

            var window = GetWindow(windowType, windowData);
            if (window is not Window<TParameter> parameterizedWindow)
            {
                Debug.LogError(
                    $"[WindowsController::Show] Window {windowType} must inherit " +
                    $"{typeof(Window<TParameter>).Name}");
                return;
            }

            if (parameterizedWindow.Data.IsPopup is false && CurrentWindow != null)
            {
                CurrentWindow.Hide();
                WindowClosed?.Invoke(CurrentWindow.Type);
            }

            parameterizedWindow.RectTransform.SetAsLastSibling();

            CurrentWindow = parameterizedWindow;
            _history.Push(parameterizedWindow);

            parameterizedWindow.Show(parameters);
            WindowOpened?.Invoke(parameterizedWindow.Type);
        }

        internal void Back()
        {
            if (CanGoBack is false)
                return;

            var currentWindow = _history.Pop();

            currentWindow.Hide();
            WindowClosed?.Invoke(currentWindow.Type);

            if (_history.Count == 0)
            {
                CurrentWindow = null;
                return;
            }

            var previousWindow = _history.Peek();
            CurrentWindow = previousWindow;

            if (previousWindow.IsVisible is false)
                previousWindow.Show();

            WindowOpened?.Invoke(previousWindow.Type);
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