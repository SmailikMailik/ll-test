using System;
using System.Collections.Generic;
using LL.Game.Cards;
using LL.Game.Items;
using LL.Presentation.Icons;
using LL.Presentation.Typography;
using LL.UI.Controls;
using LL.UI.VisualStates.Sources;
using LL.User.State.Items;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace LL.UI.Windows.Upgrade.Cards
{
    [DisallowMultipleComponent]
    internal sealed class UpgradeCardView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private SelectionStateSource _stateSource;

        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _progressLabel;
        [SerializeField] private InteractiveButton _addButton;

        private const int AddAmount = 1;
        private const int MinAmount = 0;

        internal ICard Card { get; private set; }

        internal int AvailableAmount { get; private set; }
        internal int PlannedAmount { get; private set; }

        internal Observable<Unit> Clicked => _clicked;
        internal Observable<int> AvailableAmountChanged => _availableAmountChanged;

        private readonly Subject<Unit> _clicked = new();
        private readonly Subject<int> _availableAmountChanged = new();

        private IUserItems _userItems;
        private IconCatalog<ItemId> _iconCatalog;
        private bool _isInitialized;

        [Inject]
        private void Construct(
            IUserItems userItems,
            IconCatalog<ItemId> iconCatalog)
        {
            _userItems = userItems ?? throw new ArgumentNullException(nameof(userItems));
            _iconCatalog = iconCatalog ?? throw new ArgumentNullException(nameof(iconCatalog));
        }

        internal void Initialize(ICard card)
        {
            if (_isInitialized)
                throw new InvalidOperationException($"{nameof(UpgradeCardView)} is already initialized.");

            if (card == null)
                throw new ArgumentNullException(nameof(card));

            if (string.IsNullOrWhiteSpace(card.Id.Value))
                throw new ArgumentException("Card Id cannot be empty.", nameof(card));

            if (_iconCatalog.TryGetIcon(card.Id, out var icon) is false)
                throw new KeyNotFoundException($"Missing icon for Card Id: {card.Id}");

            _isInitialized = true;

            Card = card;
            PlannedAmount = MinAmount;
            _iconImage.sprite = icon;

            SetSelected(false);

            _addButton.Clicked.Subscribe(_ => _userItems.TryAdd(Card.Id, AddAmount)).AddTo(this);
            _userItems.ObserveAmount(Card.Id).Subscribe(UpdateProgress).AddTo(this);
        }

        public void OnPointerClick(PointerEventData _)
        {
            if (AvailableAmount > MinAmount)
                _clicked.OnNext(Unit.Default);
        }

        private void OnDestroy()
        {
            _clicked.Dispose();
            _availableAmountChanged.Dispose();
        }

        internal void SetPlannedAmount(int amount)
        {
            PlannedAmount = Math.Clamp(amount, MinAmount, AvailableAmount);
            _progressLabel.text = TextFormatter.Progress(PlannedAmount, AvailableAmount);
        }

        internal void SetSelected(bool isSelected)
        {
            _stateSource.SetSelected(isSelected);
        }

        private void UpdateProgress(int availableAmount)
        {
            AvailableAmount = Math.Max(MinAmount, availableAmount);
            SetPlannedAmount(PlannedAmount);
            _addButton.SetInteractable(AvailableAmount < int.MaxValue);
            _availableAmountChanged.OnNext(AvailableAmount);
        }
    }
}