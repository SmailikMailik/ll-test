using System;
using System.Collections.Generic;
using LL.Game.Cards;
using LL.User.Core.Cards;
using LL.User.Core.Progress;
using VContainer;

namespace LL.Upgrades
{
    internal interface ICardExperienceService
    {
        bool TryGetApplication(IReadOnlyList<CardAmount> cards, out ExperienceApplication application);
        bool TryApply(IReadOnlyList<CardAmount> cards);
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

        public bool TryApply(IReadOnlyList<CardAmount> cards)
        {
            if (TryGetApplication(cards, out var application) is false)
                return false;

            var spentCards = new List<CardAmount>(cards.Count);

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

            if (_userProgress.TryAddExperience(application.GrantedExperience))
                return true;

            RestoreCards(spentCards);
            return false;
        }

        public bool TryGetApplication(IReadOnlyList<CardAmount> cards, out ExperienceApplication application)
        {
            application = default;

            if (TryCalculateExperience(cards, out var grantedExperience) is false)
                return false;

            var appliedExperience = _userProgress.GetApplicableExperience(grantedExperience);

            if (appliedExperience <= MinimumAmount)
                return false;

            application = new ExperienceApplication(grantedExperience, appliedExperience);
            return true;
        }

        private bool TryCalculateExperience(IReadOnlyList<CardAmount> cards, out int experience)
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

        private void RestoreCards(IEnumerable<CardAmount> cards)
        {
            foreach (var card in cards)
                _userCards.TryAdd(card.Id, card.Amount);
        }
    }
}