using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Cards;
using LL.Game.Items;
using LL.Game.Upgrades;
using R3;
using UnityEngine;
using VContainer;

namespace LL.UI.Windows.Views.Upgrade.Cards
{
    [DisallowMultipleComponent]
    internal sealed class UpgradeCardSelector : MonoBehaviour
    {
        [SerializeField] private UpgradeCardSlot[] _slots;

        private const int MinAmount = 0;

        internal bool HasSelection => _selectedView != null;
        internal ICard SelectedCard => _selectedView?.Card;
        internal int SelectedAvailableAmount => _selectedView?.AvailableAmount ?? MinAmount;
        internal int SelectedPlannedAmount => _selectedView?.PlannedAmount ?? MinAmount;
        internal int PlannedExperience => _slots.Sum(slot => slot.View.PlannedAmount * slot.View.Card.ExperienceAmount);

        internal Observable<Unit> Changed => _changed;

        private readonly Subject<Unit> _changed = new();

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
            _changed.AddTo(this);

            foreach (var slot in _slots)
            {
                if (_cardCatalog.TryGetCard(slot.Id, out var card) is false)
                    throw new KeyNotFoundException($"Missing card data for Card Id: {slot.Id}");

                var view = slot.View;

                view.Clicked.Subscribe(_ => Select(view)).AddTo(this);
                view.AvailableAmountChanged.Subscribe(_ => OnAmountChanged(view)).AddTo(this);

                view.Initialize(card);
            }
        }

        internal void RestoreDefaultSelection()
        {
            ClearPlan();

            foreach (var slot in _slots)
                slot.View.SetSelected(false);

            _selectedView = null;

            foreach (var slot in _slots)
            {
                Select(slot.View);

                if (HasSelection)
                    return;
            }

            _changed.OnNext(Unit.Default);
        }

        internal void SetSelectedPlannedAmount(int amount)
        {
            _selectedView?.SetPlannedAmount(amount);
        }

        internal bool CanReachExperience(int requiredExperience)
        {
            return CardExperiencePlanBuilder.CanReach(
                CreateExperienceOptions(),
                requiredExperience);
        }

        internal void SetMaxPlan(int requiredExperience)
        {
            var cards = CreateExperienceOptions();
            var plan = CardExperiencePlanBuilder.Build(cards, requiredExperience);

            ClearPlan();

            foreach (var cardAmount in plan)
            {
                var slot = _slots.First(item => item.Id.Equals(cardAmount.Id));
                slot.View.SetPlannedAmount(cardAmount.Amount);
            }

            _changed.OnNext(Unit.Default);
        }

        internal IReadOnlyList<ItemAmount> GetPlan()
        {
            var plannedCards = new List<ItemAmount>(_slots.Length);

            foreach (var slot in _slots)
            {
                if (slot.View.PlannedAmount > MinAmount)
                    plannedCards.Add(new ItemAmount(slot.Id, slot.View.PlannedAmount));
            }

            return plannedCards;
        }

        internal void ClearPlan()
        {
            foreach (var slot in _slots)
                slot.View.SetPlannedAmount(MinAmount);
        }

        private ExperienceCardOption[] CreateExperienceOptions()
        {
            return _slots
                .Select(slot => new ExperienceCardOption(
                    slot.Id,
                    slot.View.AvailableAmount,
                    slot.View.Card.ExperienceAmount))
                .ToArray();
        }

        private void Select(UpgradeCardView card)
        {
            if (card.AvailableAmount <= MinAmount)
                return;

            foreach (var slot in _slots)
            {
                var item = slot.View;
                item.SetSelected(item == card);
            }

            _selectedView = card;
            _changed.OnNext(Unit.Default);
        }

        private void OnAmountChanged(UpgradeCardView card)
        {
            if (card != _selectedView)
                return;

            if (card.AvailableAmount <= MinAmount)
            {
                card.SetSelected(false);
                _selectedView = null;
            }

            _changed.OnNext(Unit.Default);
        }
    }

    [Serializable]
    internal sealed class UpgradeCardSlot
    {
        [SerializeField] private UpgradeCardView _view;
        [SerializeField] private string _cardId;

        internal UpgradeCardView View => _view;
        internal ItemId Id => new(_cardId);
    }
}