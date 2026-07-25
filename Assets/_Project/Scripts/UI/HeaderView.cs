using LL.Extensions;
using LL.UI.Windows;
using LL.User.Core.Identity;
using LL.User.Core.Wallet;
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

        [SerializeField] private TMP_Text _userLabel;
        [SerializeField] private TMP_Text _softLabel;
        [SerializeField] private TMP_Text _hardLabel;
        [SerializeField] private TMP_Text _masterPointLabel;

        private UserIdentity _identity;
        private IUserWallet _userWallet;
        private WindowController _windowController;

        [Inject]
        private void Construct(
            UserIdentity identity,
            IUserWallet userWallet,
            WindowController windowController)
        {
            _identity = identity;
            _userWallet = userWallet;
            _windowController = windowController;
        }

        private void Start()
        {
            _backButton.Clicked.Subscribe(_ => _windowController.Back()).AddTo(this);
            _homeButton.Clicked.Subscribe(_ => Debug.LogError("OnHomeClicked")).AddTo(this);

            _userLabel.text = $"{_identity.RegionCode} {_identity.UserId}";

            _userWallet.ObserveAmount(CurrencyType.Soft)
                .Subscribe(value => _softLabel.text = value.ToNumber())
                .AddTo(this);

            _userWallet.ObserveAmount(CurrencyType.Hard)
                .Subscribe(value => _hardLabel.text = value.ToNumber())
                .AddTo(this);

            _userWallet.ObserveAmount(CurrencyType.MasterPoint)
                .Subscribe(value => _masterPointLabel.text = value.ToNumber())
                .AddTo(this);
        }
    }
}