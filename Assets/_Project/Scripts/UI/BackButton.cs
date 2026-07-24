using LL.UI.Windows;
using UnityEngine;
using VContainer;

namespace LL.UI
{
    internal sealed class BackButton : MonoBehaviour
    {
        [SerializeField] private CommonButton _button;

        private WindowController _windowController;

        [Inject]
        private void Construct(WindowController windowController)
        {
            _windowController = windowController;
        }

        private void OnEnable() => _button.Clicked += OnClicked;
        private void OnDisable() => _button.Clicked -= OnClicked;

        private void OnClicked() => _windowController.Back();
    }
}