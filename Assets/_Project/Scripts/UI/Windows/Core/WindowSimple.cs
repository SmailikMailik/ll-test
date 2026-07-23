namespace LL.UI.Windows.Core
{
    internal class WindowSimple : WindowBase
    {
        internal WindowType Type { get; private set; }
        internal WindowData Data { get; private set; }

        internal void Init(WindowType type, WindowData data)
        {
            Type = type;
            Data = data;

            Init();
        }
    }
}