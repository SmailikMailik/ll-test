using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace LL.UI.Windows
{
    internal sealed class WindowProvider
    {
        private readonly Dictionary<Type, WindowBase> _windows = new();

        private readonly WindowCatalog _catalog;
        private readonly IObjectResolver _resolver;

        [Inject]
        internal WindowProvider(WindowCatalog catalog, IObjectResolver resolver)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        internal bool TryGetOrCreate<TParameter>(Transform parent, out Window<TParameter> window)
            where TParameter : class, IWindowParameters
        {
            window = null;
            var parameterType = typeof(TParameter);

            if (_catalog.TryGetDefinition(parameterType, out var definition) is false)
            {
                Debug.LogError($"[WindowProvider::TryGetOrCreate] Window for {parameterType.Name} is not configured");
                return false;
            }

            if (definition.Prefab is not Window<TParameter> typedPrefab)
            {
                Debug.LogError($"[WindowProvider::TryGetOrCreate] Prefab for {parameterType.Name} has an invalid type");
                return false;
            }

            if (_windows.TryGetValue(parameterType, out var cachedWindow) && cachedWindow != null)
            {
                if (cachedWindow is Window<TParameter> cachedTypedWindow)
                {
                    window = cachedTypedWindow;
                    return true;
                }

                Debug.LogError(
                    $"[WindowProvider::TryGetOrCreate] Cached window for {parameterType.Name} has an invalid type",
                    cachedWindow);
                return false;
            }

            window = _resolver.Instantiate(typedPrefab, parent);
            window.name = typedPrefab.GetType().Name;
            window.Initialize(definition);

            _windows[parameterType] = window;

            return true;
        }
    }
}