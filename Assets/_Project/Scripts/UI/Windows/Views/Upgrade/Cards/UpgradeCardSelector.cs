using System;
using System.Collections.Generic;
using LL.Game.Cards;
using R3;
using UnityEngine;
using VContainer;

namespace LL.UI.Windows.Views.Upgrade.Cards
{
    [DisallowMultipleComponent]
    internal sealed class UpgradeCardSelector : MonoBehaviour
    {
        [SerializeField] private UpgradeCardSlot[] _slots;

        private const int MinimumAmount = 0;

        internal bool HasSelection => _selectedView != null;
        internal ICard SelectedCard { get; private set; }
        internal int SelectedAmount => _selectedView == null
            ? MinimumAmount
            : _selectedView.AvailableAmount;
        internal int SelectedPlannedAmount => _selectedView == null
            ? MinimumAmount
            : _selectedView.PlannedAmount;
        internal int PlannedExperience
        {
            get
            {
                var experience = MinimumAmount;

                foreach (var slot in _slots)
                {
                    var plannedAmount = slot.View.PlannedAmount;

                    if (plannedAmount <= MinimumAmount ||
                        _cardCatalog.TryGetCard(slot.Id, out var card) is false)
                    {
                        continue;
                    }

                    experience += plannedAmount * card.ExperienceAmount;
                }

                return experience;
            }
        }

        internal Observable<Unit> SelectionChanged => _selectionChanged;
        internal Observable<int> SelectedAmountChanged => _selectedAmountChanged;

        private readonly Subject<Unit> _selectionChanged = new();
        private readonly Subject<int> _selectedAmountChanged = new();
        private CardCatalog _cardCatalog;
        private UpgradeCardView _selectedView;
        private bool _isInitialized;

        [Inject]
        private void Construct(CardCatalog cardCatalog)
        {
            _cardCatalog = cardCatalog ?? throw new ArgumentNullException(nameof(cardCatalog));
        }

        internal void Initialize()
        {
            if (_isInitialized)
                throw new InvalidOperationException($"{nameof(UpgradeCardSelector)} is already initialized.");

            _isInitialized = true;

            foreach (var slot in _slots)
            {
                var card = slot.View;
                card.Clicked
                    .Subscribe(_ => Select(card))
                    .AddTo(this);
                card.AvailableAmountChanged
                    .Subscribe(_ => OnAmountChanged(card))
                    .AddTo(this);
                card.Initialize(slot.Id);
            }
        }

        private void OnDestroy()
        {
            _selectionChanged.Dispose();
            _selectedAmountChanged.Dispose();
        }

        internal void ResetSelection()
        {
            foreach (var slot in _slots)
            {
                slot.View.SetSelected(false);
                slot.View.SetPlannedAmount(MinimumAmount);
            }

            _selectedView = null;
            SelectedCard = null;

            foreach (var slot in _slots)
            {
                Select(slot.View);

                if (HasSelection)
                    return;
            }

            _selectionChanged.OnNext(Unit.Default);
        }

        internal void SetPlannedAmount(int amount)
        {
            _selectedView?.SetPlannedAmount(amount);
        }

        internal IReadOnlyList<PlannedCard> GetPlannedCards()
        {
            var plannedCards = new List<PlannedCard>(_slots.Length);

            foreach (var slot in _slots)
            {
                if (slot.View.PlannedAmount > MinimumAmount)
                    plannedCards.Add(new PlannedCard(slot.Id, slot.View.PlannedAmount));
            }

            return plannedCards;
        }

        internal void ClearPlannedAmounts()
        {
            foreach (var slot in _slots)
                slot.View.SetPlannedAmount(MinimumAmount);
        }

        private void Select(UpgradeCardView card)
        {
            if (card.AvailableAmount <= MinimumAmount ||
                _cardCatalog.TryGetCard(card.Id, out var cardData) is false)
            {
                return;
            }

            foreach (var slot in _slots)
            {
                var item = slot.View;
                item.SetSelected(item == card);
            }

            _selectedView = card;
            SelectedCard = cardData;
            _selectionChanged.OnNext(Unit.Default);
        }

        private void OnAmountChanged(UpgradeCardView card)
        {
            if (card != _selectedView)
                return;

            if (card.AvailableAmount <= MinimumAmount)
            {
                card.SetSelected(false);
                _selectedView = null;
                SelectedCard = null;
                _selectionChanged.OnNext(Unit.Default);
                return;
            }

            _selectedAmountChanged.OnNext(card.AvailableAmount);
        }
    }

    [Serializable]
    internal sealed class UpgradeCardSlot
    {
        [SerializeField] private UpgradeCardView _view;
        [SerializeField] private string _cardId;

        internal UpgradeCardView View => _view;
        internal CardId Id => new(_cardId);
    }

    internal readonly struct PlannedCard
    {
        internal CardId Id { get; }
        internal int Amount { get; }

        internal PlannedCard(CardId id, int amount)
        {
            Id = id;
            Amount = amount;
        }
    }
}