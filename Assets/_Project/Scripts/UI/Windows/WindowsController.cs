using System;
using System.Collections.Generic;
using System.Linq;
using LL.UI.Windows.Core;
using UnityEngine;
using VContainer;

namespace LL.UI.Windows
{
    internal sealed class WindowsController : MonoBehaviour
    {
        [SerializeField] private Transform _container;

        [Inject] private WindowsSettings _settings;
        [Inject] private IObjectResolver _resolver;

        internal event Action<WindowType> WindowOpened;
        internal event Action<WindowType> WindowClosed;

        internal WindowSimple CurrentWindow { get; private set; }

        private readonly Dictionary<WindowType, List<WindowSimple>> _createdWindows = new();
        private readonly Stack<WindowSimple> _history = new();

        internal void Show<TParameter>(WindowType windowType, TParameter parameters)
            where TParameter : IWindowParameters
        {
            if (windowType == WindowType.Unknown)
                return;

            if (CurrentWindow && CurrentWindow.Type == windowType)
                return;

            var window = GetWindow(windowType);
            if (window is not WindowParameterized<TParameter> windowParameterized)
            {
                Debug.LogError("[WindowsController::Show] Window must be WindowParameterized");
                return;
            }

            if (windowParameterized.Data.IsPopup is false && CurrentWindow != null)
            {
                CurrentWindow.Hide();
                WindowClosed?.Invoke(CurrentWindow.Type);
            }

            CurrentWindow = windowParameterized;
            windowParameterized.Show(parameters);
            WindowOpened?.Invoke(windowParameterized.Type);
        }

        internal void Back()
        {
            if (_history.Count < 1)
                return;

            var currWindow = _history.Pop();
            var prevWindow = _history.Peek();

            currWindow.Hide();
            WindowClosed?.Invoke(currWindow.Type);

            CurrentWindow = prevWindow;

            if (currWindow.Data.IsPopup is false)
                prevWindow.Show();

            WindowOpened?.Invoke(prevWindow.Type);
        }

        private WindowSimple GetWindow(WindowType windowType)
        {
            var window = GetCreatedWindow(windowType) ?? CreateWindow(windowType);

            window.RectTransform.SetAsLastSibling();
            _history.Push(window);

            return window;
        }

        private WindowSimple GetCreatedWindow(WindowType windowType)
        {
            return _createdWindows.TryGetValue(windowType, out var windows)
                ? windows.FirstOrDefault(window => window.IsActive is false)
                : null;
        }

        private WindowSimple CreateWindow(WindowType windowType)
        {
            var windowData = _settings.Items[windowType];
            var window = Instantiate(windowData.Prefab, _container);
            _resolver.Inject(window);

            if (_createdWindows.TryGetValue(windowType, out var windows))
                windows.Add(window);
            else
                _createdWindows.Add(windowType, new List<WindowSimple> { window });

            window.name = $"{windowType}";
            window.Init(windowType, windowData);

            return window;
        }
    }
}