using LL.Game.Items;
using LL.UI.Controls.Buttons;
using LL.UI.Typography;
using LL.UI.Windows;
using LL.User.Core.Identity;
using LL.User.Core.Items;
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
        private IUserItems _items;
        private WindowController _windowController;

        [Inject]
        private void Construct(
            UserIdentity identity,
            IUserItems items,
            WindowController windowController)
        {
            _identity = identity;
            _items = items;
            _windowController = windowController;
        }

        private void Start()
        {
            _backButton.Clicked.Subscribe(_ => _windowController.Back()).AddTo(this);
            _homeButton.Clicked.Subscribe(_ => Debug.LogError("OnHomeClicked")).AddTo(this);

            _userLabel.text = $"{_identity.RegionCode} {_identity.UserId}";

            ObserveItem(ItemIds.Soft, _softLabel);
            ObserveItem(ItemIds.Hard, _hardLabel);
            ObserveItem(ItemIds.MasterPoint, _masterPointLabel);
        }

        private void ObserveItem(ItemId id, TMP_Text label)
        {
            _items.ObserveAmount(id)
                .Subscribe(amount => label.text = TextFormatter.ItemAmount(id, amount))
                .AddTo(this);
        }
    }
}