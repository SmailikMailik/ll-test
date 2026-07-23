namespace LL.UI.Windows.Core
{
    internal class WindowParameterized<TParameter> : WindowSimple
        where TParameter : IWindowParameters
    {
        internal TParameter Parameters { get; private set; }

        internal void Show(TParameter parameters)
        {
            Parameters = parameters;
            Show();
        }
    }

    internal interface IWindowParameters { }
}