using System.Collections.Generic;

namespace LL.UI.Windows
{
    internal readonly struct WindowHistoryEntry
    {
        internal WindowBase Window { get; }
        internal IWindowParameters Parameters { get; }

        internal WindowHistoryEntry(WindowBase window, IWindowParameters parameters)
        {
            Window = window;
            Parameters = parameters;
        }
    }

    internal sealed class WindowNavigator
    {
        internal ReactiveParameter<WindowBase> CurrentWindow { get; } = new();

        internal bool CanGoBack
        {
            get
            {
                if (_history.Count == 0)
                    return false;

                var currentWindow = _history.Peek().Window;
                return currentWindow.CanClose && (_history.Count > 1 || currentWindow.Definition.IsPopup);
            }
        }

        private readonly Stack<WindowHistoryEntry> _history = new();
        private readonly HashSet<WindowBase> _knownWindows = new();

        private readonly List<WindowHistoryEntry> _visibleEntries = new();
        private readonly HashSet<WindowBase> _visibleWindows = new();

        internal void Show<TParameter>(Window<TParameter> window, TParameter parameters)
            where TParameter : class, IWindowParameters
        {
            _history.Push(new WindowHistoryEntry(window, parameters));
            _knownWindows.Add(window);

            try
            {
                ApplyVisibleState(window);
            }
            catch
            {
                _history.Pop();
                ApplyVisibleState();
                throw;
            }

            CurrentWindow.SetValue(window, true);
        }

        internal bool Back()
        {
            if (CanGoBack is false)
                return false;

            var currentEntry = _history.Pop();

            try
            {
                ApplyVisibleState();
            }
            catch
            {
                _history.Push(currentEntry);
                ApplyVisibleState(currentEntry.Window);
                CurrentWindow.Value = currentEntry.Window;
                throw;
            }

            CurrentWindow.SetValue(_history.Count > 0 ? _history.Peek().Window : null, true);

            return true;
        }

        private void ApplyVisibleState(WindowBase forceRefresh = null)
        {
            CollectVisibleEntries();

            foreach (var window in _knownWindows)
            {
                if (_visibleWindows.Contains(window) is false)
                    window.Hide();
            }

            for (var i = _visibleEntries.Count - 1; i >= 0; i--)
            {
                var entry = _visibleEntries[i];
                var window = entry.Window;

                if (window == forceRefresh ||
                    window.IsVisible is false ||
                    ReferenceEquals(window.CurrentParameters, entry.Parameters) is false)
                {
                    window.Hide();
                    window.Show(entry.Parameters);
                }

                window.RectTransform.SetAsLastSibling();
            }
        }

        private void CollectVisibleEntries()
        {
            _visibleEntries.Clear();
            _visibleWindows.Clear();

            foreach (var entry in _history)
            {
                if (_visibleWindows.Add(entry.Window))
                    _visibleEntries.Add(entry);

                if (entry.Window.Definition.IsPopup is false)
                    break;
            }
        }
    }
}