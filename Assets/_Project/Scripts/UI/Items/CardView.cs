using System;
using System.Collections.Generic;
using LL.Game.Cards;
using LL.Presentation.Icons;
using LL.UI.Controls.Buttons;
using LL.UI.Formatting;
using LL.UI.VisualStates.Sources;
using LL.User.Core.Cards;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace LL.UI.Items
{
    [DisallowMultipleComponent]
    internal sealed class CardView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private SelectionStateSource _stateSource;

        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _progressLabel;
        [SerializeField] private InteractiveButton _addButton;

        private const int AddAmount = 1;
        private const int MinimumAmount = 0;

        internal CardId Id { get; private set; }
        internal int AvailableAmount { get; private set; }

        private IUserCards _userCards;
        private IIconProvider<CardId> _iconProvider;

        private readonly Subject<Unit> _clicked = new();
        private readonly Subject<int> _availableAmountChanged = new();

        private int _plannedAmount;
        private bool _isInitialized;

        internal Observable<Unit> Clicked => _clicked;
        internal Observable<int> AvailableAmountChanged => _availableAmountChanged;

        [Inject]
        private void Construct(
            IUserCards userCards,
            IIconProvider<CardId> iconProvider)
        {
            _userCards = userCards ?? throw new ArgumentNullException(nameof(userCards));
            _iconProvider = iconProvider ?? throw new ArgumentNullException(nameof(iconProvider));
        }

        internal void Initialize(CardId id)
        {
            if (_isInitialized)
                throw new InvalidOperationException($"{nameof(CardView)} is already initialized.");

            if (id.IsEmpty)
                throw new ArgumentException("Card Id cannot be empty.", nameof(id));

            if (_iconProvider.TryGetIcon(id, out var icon) is false)
                throw new KeyNotFoundException($"Missing icon for Card Id: {id}");

            _isInitialized = true;

            Id = id;
            _plannedAmount = MinimumAmount;
            _iconImage.sprite = icon;

            SetSelected(false);

            _addButton.Clicked
                .Subscribe(_ => _userCards.TryAdd(Id, AddAmount))
                .AddTo(this);

            _userCards
                .ObserveAmount(id)
                .Subscribe(UpdateProgress)
                .AddTo(this);
        }

        public void OnPointerClick(PointerEventData _)
        {
            if (AvailableAmount > MinimumAmount)
                _clicked.OnNext(Unit.Default);
        }

        private void OnDestroy()
        {
            _clicked.Dispose();
            _availableAmountChanged.Dispose();
        }

        internal void SetPlannedAmount(int amount)
        {
            _plannedAmount = Math.Clamp(amount, MinimumAmount, AvailableAmount);
            _progressLabel.text = TextFormatter.Progress(_plannedAmount, AvailableAmount);
        }

        internal void SetSelected(bool isSelected)
        {
            _stateSource.SetSelected(isSelected);
        }

        private void UpdateProgress(int availableAmount)
        {
            AvailableAmount = Math.Max(MinimumAmount, availableAmount);
            SetPlannedAmount(_plannedAmount);
            _addButton.SetInteractable(AvailableAmount < int.MaxValue);
            _availableAmountChanged.OnNext(AvailableAmount);
        }
    }
}