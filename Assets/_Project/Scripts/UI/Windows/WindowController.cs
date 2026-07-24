using UnityEngine;
using VContainer;

namespace LL.UI.Windows
{
    internal sealed class WindowController : MonoBehaviour
    {
        [SerializeField] private Transform _container;

        internal IReadOnlyReactiveParameter<WindowBase> CurrentWindow => _navigator.CurrentWindow;
        internal bool CanGoBack => _navigator.CanGoBack;

        private WindowProvider _provider;
        private WindowNavigator _navigator;

        private void Awake()
        {
            if (_container == null)
            {
                Debug.LogError(
                    "[WindowController::Awake] Window container is not assigned",
                    this);
            }
        }

        [Inject]
        private void Construct(WindowProvider provider, WindowNavigator navigator)
        {
            _provider = provider;
            _navigator = navigator;
        }

        internal void Show<TParameter>(TParameter parameters)
            where TParameter : class, IWindowParameters
        {
            if (parameters is null)
            {
                Debug.LogError(
                    "[WindowController::Show] Parameters are null",
                    this);
                return;
            }

            if (_container == null)
                return;

            if (_provider.TryGetOrCreate<TParameter>(_container, out var window) is false)
                return;

            _navigator.Show(window, parameters);
        }

        internal bool Back() => _navigator.Back();
    }
}