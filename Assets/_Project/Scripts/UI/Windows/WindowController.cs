using UnityEngine;
using VContainer;

namespace LL.UI.Windows
{
    internal sealed class WindowController : MonoBehaviour
    {
        [SerializeField] private Transform _container;

        internal ReactiveParameter<WindowBase> CurrentWindow => _navigator.CurrentWindow;

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

            if (CurrentWindow.Value?.ParameterType == typeof(TParameter))
                return;

            if (_provider.TryGet<TParameter>(_container, out var window) is false)
                return;

            if (_navigator.TryShow(window, parameters) is false)
            {
                Debug.LogWarning(
                    $"[WindowController::Show] Window for {typeof(TParameter).Name} " +
                    "is already in history. " +
                    "Use Back to return to it.",
                    this);
                return;
            }
        }

        internal void Back() => _navigator.Back();
    }
}