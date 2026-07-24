using LL.Extensions;
using LL.UI.Windows;
using TMPro;
using UnityEngine;
using VContainer;

namespace LL.UI
{
    internal sealed class HeaderView : MonoBehaviour
    {
        [SerializeField] private CommonButton _backButton;
        [SerializeField] private CommonButton _homeButton;

        [SerializeField] private TMP_Text _softLabel;
        [SerializeField] private TMP_Text _hardLabel;
        [SerializeField] private TMP_Text _masterPointLabel;

        private UserData _userData;
        private WindowController _windowController;

        [Inject]
        private void Construct(UserData userData, WindowController windowController)
        {
            _userData = userData;
            _windowController = windowController;
        }

        private void Start()
        {
            _backButton.Clicked += OnBackClicked;
            _homeButton.Clicked += OnHomeClicked;

            _userData.SoftAmount.Changed += OnSoftAmountChanged;
            _userData.HardAmount.Changed += OnHardAmountChanged;
            _userData.MasterPointAmount.Changed += OnMasterPointAmountChanged;

            Refresh();
        }

        private void OnDestroy()
        {
            _backButton.Clicked -= OnBackClicked;
            _homeButton.Clicked -= OnHomeClicked;

            if (_userData == null)
                return;

            _userData.SoftAmount.Changed -= OnSoftAmountChanged;
            _userData.HardAmount.Changed -= OnHardAmountChanged;
            _userData.MasterPointAmount.Changed -= OnMasterPointAmountChanged;
        }

        private void Refresh()
        {
            OnSoftAmountChanged(_userData.SoftAmount.Value);
            OnHardAmountChanged(_userData.HardAmount.Value);
            OnMasterPointAmountChanged(_userData.MasterPointAmount.Value);
        }

        private void OnBackClicked() => _windowController.Back();
        private void OnHomeClicked() => Debug.LogError("OnHomeClicked");

        private void OnSoftAmountChanged(int value) => _softLabel.text = value.ToNumber();
        private void OnHardAmountChanged(int value) => _hardLabel.text = value.ToNumber();
        private void OnMasterPointAmountChanged(int value) => _masterPointLabel.text = value.ToNumber();
    }
}