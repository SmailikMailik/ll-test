namespace LL.UI.Windows
{
    internal readonly struct WindowHistoryItem
    {
        internal WindowBase Window { get; }
        internal IWindowParameters Parameters { get; }

        internal WindowHistoryItem(WindowBase window, IWindowParameters parameters)
        {
            Window = window;
            Parameters = parameters;
        }
    }
}