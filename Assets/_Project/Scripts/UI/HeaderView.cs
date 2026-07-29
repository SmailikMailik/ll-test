using LL.Game.Items;
using LL.Presentation.Items;
using LL.UI.Controls;
using LL.UI.Windows;
using LL.User.State.Items;
using LL.User.Snapshots;
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

        private UserIdentitySnapshot _identity;
        private IUserItems _items;
        private WindowController _windowController;

        [Inject]
        private void Construct(
            UserIdentitySnapshot identity,
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
            _homeButton.Clicked.Subscribe(_ => Debug.LogError("Дом в сделку не входил")).AddTo(this);

            _userLabel.text = $"{_identity.RegionCode} {_identity.UserId}";

            ObserveItem(ItemIds.Soft, _softLabel);
            ObserveItem(ItemIds.Hard, _hardLabel);
            ObserveItem(ItemIds.MasterPoint, _masterPointLabel);
        }

        private void ObserveItem(ItemId id, TMP_Text label)
        {
            _items.ObserveAmount(id)
                .Subscribe(amount => label.text = ItemAmountFormatter.Format(id, amount))
                .AddTo(this);
        }
    }
}