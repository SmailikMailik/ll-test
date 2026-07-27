using System;
using System.Collections.Generic;
using LL.Game.Cards;
using LL.Presentation.Icons;
using LL.UI.Controls.Buttons;
using LL.UI.Formatting;
using LL.UI.VisualStates.Sources;
using LL.User.Core.Cards;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace LL.UI.Items
{
    [DisallowMultipleComponent]
    internal sealed class CardView : MonoBehaviour
    {
        [Required] [SerializeField] private SelectionStateSource _stateSource;

        [SerializeField] private Image _iconImage;
        [SerializeField] private InteractiveButton _addButton;
        [SerializeField] private TMP_Text _progressLabel;

        [SerializeField] private string _cardId;

        internal CardId Id { get; private set; }

        internal int AvailableAmount { get; private set; }
        private IUserCards _userCards;
        private IIconProvider<CardId> _iconProvider;
        private IDisposable _amountSubscription;
        private readonly Subject<Unit> _clicked = new();
        private readonly Subject<int> _availableAmountChanged = new();
        private int _plannedAmount;

        internal bool IsSelected => _stateSource.IsSelected;
        internal Observable<Unit> Clicked => _clicked;
        internal Observable<int> AvailableAmountChanged => _availableAmountChanged;

        [Inject]
        private void Construct(
            IUserCards userCards,
            IIconProvider<CardId> iconProvider)
        {
            _userCards = userCards ?? throw new ArgumentNullException(nameof(userCards));
            _iconProvider = iconProvider ?? throw new ArgumentNullException(nameof(iconProvider));

            _addButton.Clicked
                .Subscribe(_ => TrySelect())
                .AddTo(this);

            UpdateView(new CardId(_cardId));
        }

        private void OnDestroy()
        {
            _amountSubscription?.Dispose();
            _clicked.Dispose();
            _availableAmountChanged.Dispose();
        }

        private void UpdateView(CardId id)
        {
            if (id.IsEmpty)
                throw new ArgumentException("Card ID cannot be empty.", nameof(id));

            if (_iconProvider.TryGetIcon(id, out var icon) is false)
                throw new KeyNotFoundException($"Missing icon for card ID: {id}");

            SetSelected(false);

            Id = id;
            _plannedAmount = 0;
            _iconImage.sprite = icon;

            _amountSubscription?.Dispose();
            _amountSubscription = _userCards
                .ObserveAmount(id)
                .Subscribe(UpdateProgress);
        }

        internal void SetPlannedAmount(int amount)
        {
            _plannedAmount = Math.Clamp(amount, 0, AvailableAmount);
            ShowProgress();
        }

        internal void SetSelected(bool isSelected)
        {
            _stateSource.SetSelected(isSelected);
        }

        private void TrySelect()
        {
            if (AvailableAmount > 0)
                _clicked.OnNext(Unit.Default);
        }

        private void UpdateProgress(int availableAmount)
        {
            AvailableAmount = Math.Max(0, availableAmount);
            _plannedAmount = Math.Min(_plannedAmount, AvailableAmount);
            _addButton.SetInteractable(AvailableAmount > 0);
            ShowProgress();
            _availableAmountChanged.OnNext(AvailableAmount);
        }

        private void ShowProgress()
        {
            _progressLabel.text = TextFormatter.Progress(_plannedAmount, AvailableAmount);
        }
    }
}