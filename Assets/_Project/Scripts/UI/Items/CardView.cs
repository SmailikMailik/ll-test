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
        [Required]
        [SerializeField] private SelectionStateSource _stateSource;

        [SerializeField] private Image _iconImage;
        [SerializeField] private InteractiveButton _addButton;
        [SerializeField] private TMP_Text _progressLabel;

        [SerializeField] private string _cardId;
        [Min(0)]
        [SerializeField] private int _plannedAmount;

        private IUserCards _userCards;
        private IIconProvider<CardId> _iconProvider;
        private IDisposable _amountSubscription;
        private CardId _id;
        private bool _isInitialized;

        internal bool IsSelected => _stateSource.IsSelected;

        [Inject]
        private void Construct(
            IUserCards userCards,
            IIconProvider<CardId> iconProvider)
        {
            _userCards = userCards ?? throw new ArgumentNullException(nameof(userCards));
            _iconProvider = iconProvider ?? throw new ArgumentNullException(nameof(iconProvider));

            _addButton.Clicked
                .Subscribe(_ => AddCard())
                .AddTo(this);
        }

        private void Start()
        {
            if (_isInitialized is false)
                UpdateView(new CardId(_cardId), _plannedAmount);
        }

        private void OnDestroy()
        {
            _amountSubscription?.Dispose();
        }

        internal void UpdateView(CardId id, int plannedAmount)
        {
            if (id.IsEmpty)
                throw new ArgumentException("Card ID cannot be empty.", nameof(id));

            if (_iconProvider.TryGetIcon(id, out var icon) is false)
                throw new KeyNotFoundException($"Missing icon for card ID: {id}");

            _isInitialized = true;
            SetSelected(false);
            _id = id;
            _plannedAmount = Math.Max(0, plannedAmount);
            _iconImage.sprite = icon;

            _amountSubscription?.Dispose();
            _amountSubscription = _userCards
                .ObserveAmount(id)
                .Subscribe(UpdateProgress);
        }

        internal void SetSelected(bool isSelected)
        {
            _stateSource.SetSelected(isSelected);
        }

        private void AddCard()
        {
            _userCards.TryAdd(_id, 1);
        }

        private void UpdateProgress(int availableAmount)
        {
            _progressLabel.text = TextFormatter.Progress(availableAmount, _plannedAmount);
        }
    }
}