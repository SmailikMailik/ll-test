using LL.Game.Currencies;
using LL.UI.Controls.Buttons;
using LL.UI.Typography;
using LL.UI.Windows;
using LL.User.Core.Identity;
using LL.User.Core.Wallet;
using R3;
using TMPro;
using UnityEngine;
using VContainer;

namespace LL.UI
{
    [DisallowMultipleComponent]
    internal sealed class HeaderView : MonoBehaviour
    {
        [SerializeField] private InteractiveButton _backButton;
        [SerializeField] private InteractiveButton _homeButton;

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

            ObserveCurrency(CurrencyIds.Soft, _softLabel);
            ObserveCurrency(CurrencyIds.Hard, _hardLabel);
            ObserveCurrency(CurrencyIds.MasterPoint, _masterPointLabel);
        }

        private void ObserveCurrency(CurrencyId id, TMP_Text label)
        {
            _userWallet.ObserveAmount(id)
                .Subscribe(amount => label.text = TextFormatter.CurrencyAmount(id, amount))
                .AddTo(this);
        }
    }
}