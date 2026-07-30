using System;

namespace LL.UI.Windows
{
    internal sealed class WindowDefinition
    {
        internal Type ParameterType => Prefab.ParameterType;

        internal WindowBase Prefab { get; }
        internal bool IsPopup { get; }

        internal WindowDefinition(WindowBase prefab, bool isPopup)
        {
            Prefab = prefab ?? throw new ArgumentNullException(nameof(prefab));
            IsPopup = isPopup;
        }
    }
}