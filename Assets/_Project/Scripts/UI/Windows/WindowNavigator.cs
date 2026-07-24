using System.Collections.Generic;

namespace LL.UI.Windows
{
    internal sealed class WindowNavigator
    {
        internal ReactiveParameter<WindowBase> CurrentWindow { get; } = new();
        internal bool CanGoBack => _history.Count > 1 || CurrentWindow.Value?.Definition.IsPopup == true;

        private readonly Stack<WindowBase> _history = new();

        internal bool TryShow<TParameter>(Window<TParameter> window, TParameter parameters)
            where TParameter : class, IWindowParameters
        {
            if (_history.Contains(window))
                return false;

            if (window.Definition.IsPopup is false && CurrentWindow.Value != null)
                CurrentWindow.Value.Hide();

            window.RectTransform.SetAsLastSibling();
            _history.Push(window);

            window.Show(parameters);
            CurrentWindow.Value = window;

            return true;
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
    }
}