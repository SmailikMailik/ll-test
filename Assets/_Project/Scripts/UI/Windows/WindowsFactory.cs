using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace LL.UI.Windows
{
    internal sealed class WindowsFactory
    {
        private readonly IObjectResolver _resolver;

        [Inject]
        internal WindowsFactory(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        internal WindowBase Create(WindowBase prefab, Transform parent)
        {
            return _resolver.Instantiate(prefab, parent);
        }
    }
}