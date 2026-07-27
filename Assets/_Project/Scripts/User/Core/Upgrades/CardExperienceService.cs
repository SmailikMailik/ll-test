using System;
using System.Collections.Generic;
using LL.Game.Cards;
using LL.User.Core.Cards;
using LL.User.Core.Progress;
using VContainer;

namespace LL.User.Core.Upgrades
{
    internal interface ICardExperienceService
    {
        bool TryApply(IReadOnlyList<CardStack> cards);
    }

    internal sealed class CardExperienceService : ICardExperienceService
    {
        private const int MinimumAmount = 0;

        private readonly IUserCards _userCards;
        private readonly IUserProgress _userProgress;
        private readonly CardCatalog _cardCatalog;

        [Inject]
        internal CardExperienceService(
            IUserCards userCards,
            IUserProgress userProgress,
            CardCatalog cardCatalog)
        {
            _userCards = userCards ?? throw new ArgumentNullException(nameof(userCards));
            _userProgress = userProgress ?? throw new ArgumentNullException(nameof(userProgress));
            _cardCatalog = cardCatalog ?? throw new ArgumentNullException(nameof(cardCatalog));
        }

        public bool TryApply(IReadOnlyList<CardStack> cards)
        {
            if (TryCalculateExperience(cards, out var experience) is false ||
                _userProgress.CanAddExperience(experience) is false)
            {
                return false;
            }

            var spentCards = new List<CardStack>(cards.Count);

            foreach (var card in cards)
            {
                if (_userCards.TrySpend(card.Id, card.Amount))
                {
                    spentCards.Add(card);
                    continue;
                }

                RestoreCards(spentCards);
                return false;
            }

            if (_userProgress.TryAddExperience(experience))
                return true;

            RestoreCards(spentCards);
            return false;
        }

        private bool TryCalculateExperience(
            IReadOnlyList<CardStack> cards,
            out int experience)
        {
            experience = MinimumAmount;

            if (cards == null || cards.Count == MinimumAmount)
                return false;

            foreach (var stack in cards)
            {
                if (stack == null ||
                    stack.Amount <= MinimumAmount ||
                    _cardCatalog.TryGetCard(stack.Id, out var card) is false ||
                    card.ExperienceAmount <= MinimumAmount)
                {
                    return false;
                }

                var availableExperience = int.MaxValue - experience;
                var maximumAmount = availableExperience / card.ExperienceAmount;

                if (stack.Amount > maximumAmount)
                    return false;

                experience += stack.Amount * card.ExperienceAmount;
            }

            return experience > MinimumAmount;
        }

        private void RestoreCards(IEnumerable<CardStack> cards)
        {
            foreach (var card in cards)
                _userCards.TryAdd(card.Id, card.Amount);
        }
    }
}