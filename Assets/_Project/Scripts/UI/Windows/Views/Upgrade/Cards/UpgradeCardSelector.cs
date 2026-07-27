using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Cards;
using LL.Upgrades;
using LL.User.Core.Cards;
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
        internal ICard SelectedCard => _selectedView?.Card;
        internal int SelectedAvailableAmount => _selectedView?.AvailableAmount ?? MinimumAmount;
        internal int SelectedPlannedAmount => _selectedView?.PlannedAmount ?? MinimumAmount;
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

                view.Clicked
                    .Subscribe(_ => Select(view))
                    .AddTo(this);
                view.AvailableAmountChanged
                    .Subscribe(_ => OnAmountChanged(view))
                    .AddTo(this);

                view.Initialize(card);
            }
        }

        internal void Reset()
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

        internal bool CanPlanExperience(int maximumExperience)
        {
            return _slots.Any(slot =>
                slot.View.AvailableAmount > MinimumAmount &&
                slot.View.Card.ExperienceAmount <= maximumExperience);
        }

        internal void SetMaximumPlan(int maximumExperience)
        {
            var cards = _slots
                .Select(slot => new ExperienceCardOption(
                    slot.Id,
                    slot.View.AvailableAmount,
                    slot.View.Card.ExperienceAmount))
                .ToArray();
            var plan = CardExperiencePlanBuilder.Build(cards, maximumExperience);

            ClearPlan();

            foreach (var stack in plan)
            {
                var slot = _slots.First(item => item.Id.Equals(stack.Id));
                slot.View.SetPlannedAmount(stack.Amount);
            }

            _changed.OnNext(Unit.Default);
        }

        internal IReadOnlyList<CardStack> GetPlan()
        {
            var plannedCards = new List<CardStack>(_slots.Length);

            foreach (var slot in _slots)
            {
                if (slot.View.PlannedAmount > MinimumAmount)
                    plannedCards.Add(new CardStack(slot.Id, slot.View.PlannedAmount));
            }

            return plannedCards;
        }

        internal void ClearPlan()
        {
            foreach (var slot in _slots)
                slot.View.SetPlannedAmount(MinimumAmount);
        }

        private void Select(UpgradeCardView card)
        {
            if (card.AvailableAmount <= MinimumAmount)
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

            if (card.AvailableAmount <= MinimumAmount)
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
        internal CardId Id => new(_cardId);
    }
}