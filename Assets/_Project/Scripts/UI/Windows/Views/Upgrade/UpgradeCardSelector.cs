using System;
using LL.Game.Cards;
using LL.UI.Items;
using R3;
using UnityEngine;
using VContainer;

namespace LL.UI.Windows.Views.Upgrade
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
        internal Observable<Unit> SelectionChanged => _selectionChanged;
        internal Observable<int> SelectedAmountChanged => _selectedAmountChanged;

        private readonly Subject<Unit> _selectionChanged = new();
        private readonly Subject<int> _selectedAmountChanged = new();
        private CardCatalog _cardCatalog;
        private CardView _selectedView;
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

        private void Select(CardView card)
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
                item.SetPlannedAmount(MinimumAmount);
            }

            _selectedView = card;
            SelectedCard = cardData;
            _selectionChanged.OnNext(Unit.Default);
        }

        private void OnAmountChanged(CardView card)
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
        [SerializeField] private CardView _view;
        [SerializeField] private string _cardId;

        internal CardView View => _view;
        internal CardId Id => new(_cardId);
    }
}