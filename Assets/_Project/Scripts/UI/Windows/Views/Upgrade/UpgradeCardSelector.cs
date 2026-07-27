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
        [SerializeField] private CardView[] _cards;

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

        [Inject]
        private void Construct(CardCatalog cardCatalog)
        {
            _cardCatalog = cardCatalog ?? throw new ArgumentNullException(nameof(cardCatalog));

            foreach (var card in _cards)
            {
                var capturedCard = card;
                capturedCard.Clicked
                    .Subscribe(_ => Select(capturedCard))
                    .AddTo(this);
                capturedCard.AvailableAmountChanged
                    .Subscribe(_ => OnAmountChanged(capturedCard))
                    .AddTo(this);
            }
        }

        private void OnDestroy()
        {
            _selectionChanged.Dispose();
            _selectedAmountChanged.Dispose();
        }

        internal void ResetSelection()
        {
            foreach (var card in _cards)
            {
                card.SetSelected(false);
                card.SetPlannedAmount(MinimumAmount);
            }

            _selectedView = null;
            SelectedCard = null;
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

            foreach (var item in _cards)
            {
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
}