using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Cards;
using R3;
using VContainer;

namespace LL.User.Core.Cards
{
    internal sealed class UserCards : IUserCards, IDisposable
    {
        private readonly IReadOnlyDictionary<CardId, ReactiveProperty<int>> _amounts;

        [Inject]
        internal UserCards(CardsInitialData initialData)
        {
            if (initialData == null)
                throw new ArgumentNullException(nameof(initialData));

            _amounts = initialData.Amounts.ToDictionary(
                stack => stack.Id,
                stack => new ReactiveProperty<int>(stack.Amount));
        }

        public Observable<int> ObserveAmount(CardId id)
        {
            if (TryGetAmount(id, out var amount))
                return amount;

            throw new KeyNotFoundException($"Unknown card ID: {id}");
        }

        public bool CanAdd(CardId id, int amount)
        {
            return TryGetAmount(id, out var currentAmount) &&
                   amount > 0 &&
                   currentAmount.Value <= int.MaxValue - amount;
        }

        public bool TryAdd(CardId id, int amount)
        {
            if (CanAdd(id, amount) is false)
                return false;

            TryGetAmount(id, out var currentAmount);
            currentAmount.Value += amount;
            return true;
        }

        public bool TrySpend(CardId id, int amount)
        {
            if (amount <= 0 ||
                TryGetAmount(id, out var currentAmount) is false ||
                currentAmount.Value < amount)
            {
                return false;
            }

            currentAmount.Value -= amount;
            return true;
        }

        public void Dispose()
        {
            foreach (var amount in _amounts.Values)
                amount.Dispose();
        }

        private bool TryGetAmount(CardId id, out ReactiveProperty<int> amount)
        {
            if (id.IsEmpty)
            {
                amount = null;
                return false;
            }

            return _amounts.TryGetValue(id, out amount);
        }
    }
}