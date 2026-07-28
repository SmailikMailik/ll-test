using System;
using System.Collections.Generic;
using LL.Game.Cards;
using LL.User.Core.Amounts;
using LL.User.Core.Progress;
using VContainer;

namespace LL.Upgrades
{
    internal interface ICardExperienceService
    {
        bool TryGetApplication(IReadOnlyList<Amount<CardId>> cards, out ExperienceApplication application);
        bool TryApply(IReadOnlyList<Amount<CardId>> cards);
    }

    internal sealed class CardExperienceService : ICardExperienceService
    {
        private const int MinimumAmount = 0;

        private readonly IUserAmounts<CardId> _userCards;
        private readonly IUserProgress _userProgress;
        private readonly CardCatalog _cardCatalog;

        [Inject]
        internal CardExperienceService(
            IUserAmounts<CardId> userCards,
            IUserProgress userProgress,
            CardCatalog cardCatalog)
        {
            _userCards = userCards ?? throw new ArgumentNullException(nameof(userCards));
            _userProgress = userProgress ?? throw new ArgumentNullException(nameof(userProgress));
            _cardCatalog = cardCatalog ?? throw new ArgumentNullException(nameof(cardCatalog));
        }

        public bool TryApply(IReadOnlyList<Amount<CardId>> cards)
        {
            if (TryGetApplication(cards, out var application) is false)
                return false;

            var spentCards = new List<Amount<CardId>>(cards.Count);

            foreach (var card in cards)
            {
                if (_userCards.TrySpend(card.Id, card.Value))
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

        public bool TryGetApplication(
            IReadOnlyList<Amount<CardId>> cards,
            out ExperienceApplication application)
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

        private bool TryCalculateExperience(IReadOnlyList<Amount<CardId>> cards, out int experience)
        {
            experience = MinimumAmount;

            if (cards == null || cards.Count == MinimumAmount)
                return false;

            foreach (var cardAmount in cards)
            {
                if (cardAmount.Value <= MinimumAmount ||
                    _cardCatalog.TryGetCard(cardAmount.Id, out var card) is false ||
                    card.ExperienceAmount <= MinimumAmount)
                {
                    return false;
                }

                var availableExperience = int.MaxValue - experience;
                var maximumAmount = availableExperience / card.ExperienceAmount;

                if (cardAmount.Value > maximumAmount)
                    return false;

                experience += cardAmount.Value * card.ExperienceAmount;
            }

            return experience > MinimumAmount;
        }

        private void RestoreCards(IEnumerable<Amount<CardId>> cards)
        {
            foreach (var card in cards)
                _userCards.TryAdd(card.Id, card.Value);
        }
    }
}