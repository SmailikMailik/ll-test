using LL.Extensions;
using LL.UI.Windows;
using R3;
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
            _backButton.Clicked.Subscribe(_ => _windowController.Back()).AddTo(this);
            _homeButton.Clicked.Subscribe(_ => Debug.LogError("OnHomeClicked")).AddTo(this);

            _userData.SoftAmount.Subscribe(value => _softLabel.text = value.ToNumber()).AddTo(this);
            _userData.HardAmount.Subscribe(value => _hardLabel.text = value.ToNumber()).AddTo(this);
            _userData.MasterPointAmount.Subscribe(value => _masterPointLabel.text = value.ToNumber()).AddTo(this);
        }
    }
}