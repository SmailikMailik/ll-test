using LL.UI.Windows;
using UnityEngine;
using VContainer;

namespace LL.UI
{
    internal sealed class BackButton : MonoBehaviour
    {
        [SerializeField] private CommonButton _button;

        private WindowsController _windowController;

        [Inject]
        private void Construct(WindowsController windowsController)
        {
            _windowController = windowsController;
        }

        private void OnEnable() => _button.Clicked += OnClicked;
        private void OnDisable() => _button.Clicked -= OnClicked;

        private void OnClicked() => _windowController.CurrentWindow?.Back();
    }
}