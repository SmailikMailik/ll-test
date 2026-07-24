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
        private readonly Dictionary<Type, WindowDefinition> _definitions = new();

        private readonly WindowCatalog _catalog;
        private readonly IObjectResolver _resolver;

        [Inject]
        internal WindowProvider(WindowCatalog catalog, IObjectResolver resolver)
        {
            _catalog = catalog;
            _resolver = resolver;

            BuildDefinitions();
        }

        internal bool TryGetOrCreate<TParameter>(Transform parent, out Window<TParameter> window)
            where TParameter : class, IWindowParameters
        {
            window = null;
            var parameterType = typeof(TParameter);

            if (_definitions.TryGetValue(parameterType, out var definition) is false)
            {
                Debug.LogError(
                    $"[WindowProvider::TryGet] Window for {parameterType.Name} is not configured",
                    _catalog);
                return false;
            }

            if (definition.Prefab is not Window<TParameter> typedPrefab)
            {
                Debug.LogError(
                    $"[WindowProvider::TryGet] Prefab for {parameterType.Name} has an invalid type",
                    _catalog);
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
                    $"[WindowProvider::TryGet] Cached window for {parameterType.Name} has an invalid type",
                    cachedWindow);
                return false;
            }

            window = _resolver.Instantiate(typedPrefab, parent);
            window.name = typedPrefab.GetType().Name;
            window.Initialize(definition);

            _windows[parameterType] = window;

            return true;
        }

        private void BuildDefinitions()
        {
            foreach (var definition in _catalog.Definitions)
            {
                if (definition?.Prefab == null)
                {
                    Debug.LogError(
                        "[WindowProvider::BuildDefinitions] Window prefab is not assigned",
                        _catalog);
                    continue;
                }

                var parameterType = definition.Prefab.ParameterType;

                if (_definitions.ContainsKey(parameterType))
                {
                    Debug.LogError(
                        $"[WindowProvider::BuildDefinitions] Multiple windows use " +
                        $"{parameterType.Name}",
                        _catalog);
                    continue;
                }

                _definitions.Add(parameterType, definition);
            }
        }
    }
}