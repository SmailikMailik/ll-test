using System;
using UnityEngine;
using VContainer;

namespace LL.UI.Windows
{
    [DisallowMultipleComponent]
    internal sealed class WindowController : MonoBehaviour
    {
        [SerializeField] private Transform _container;

        private WindowProvider _provider;
        private WindowNavigator _navigator;

        [Inject]
        private void Construct(
            WindowProvider provider,
            WindowNavigator navigator)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
        }

        internal void Show<TParameter>(TParameter parameters)
            where TParameter : class, IWindowParameters
        {
            if (TryGetWindow(parameters, out var window))
                _navigator.Show(window, parameters);
        }

        internal void Replace<TParameter>(TParameter parameters)
            where TParameter : class, IWindowParameters
        {
            if (TryGetWindow(parameters, out var window))
                _navigator.Replace(window, parameters);
        }

        internal bool Back() => _navigator.Back();

        private bool TryGetWindow<TParameter>(TParameter parameters, out Window<TParameter> window)
            where TParameter : class, IWindowParameters
        {
            window = null;

            if (parameters is null)
            {
                Debug.LogError(
                    "[WindowController::TryGetWindow] Parameters are null",
                    this);
                return false;
            }

            if (_container == null)
            {
                Debug.LogError(
                    "[WindowController::TryGetWindow] Window container is not assigned",
                    this);
                return false;
            }

            return _provider.TryGetOrCreate(_container, out window);
        }
    }
}