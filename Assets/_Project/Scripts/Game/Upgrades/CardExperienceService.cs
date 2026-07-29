using System;
using System.Collections.Generic;
using LL.Game.Cards;
using LL.Game.Items;
using LL.User.State.Items;
using LL.User.State.Progress;
using VContainer;

namespace LL.Game.Upgrades
{
    internal interface ICardExperienceService
    {
        bool TryGetApplication(IReadOnlyList<ItemAmount> cards, out ExperienceApplication application);
        bool TryApply(IReadOnlyList<ItemAmount> cards);
    }

    internal sealed class CardExperienceService : ICardExperienceService
    {
        private const int MinimumAmount = 0;

        private readonly IUserItems _userItems;
        private readonly IUserProgress _userProgress;
        private readonly CardCatalog _cardCatalog;

        [Inject]
        internal CardExperienceService(
            IUserItems userItems,
            IUserProgress userProgress,
            CardCatalog cardCatalog)
        {
            _userItems = userItems ?? throw new ArgumentNullException(nameof(userItems));
            _userProgress = userProgress ?? throw new ArgumentNullException(nameof(userProgress));
            _cardCatalog = cardCatalog ?? throw new ArgumentNullException(nameof(cardCatalog));
        }

        public bool TryApply(IReadOnlyList<ItemAmount> cards)
        {
            if (TryGetApplication(cards, out var application) is false)
                return false;

            var spentCards = new List<ItemAmount>(cards.Count);

            foreach (var card in cards)
            {
                if (_userItems.TrySpend(card.Id, card.Amount))
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
            IReadOnlyList<ItemAmount> cards,
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

        private bool TryCalculateExperience(IReadOnlyList<ItemAmount> cards, out int experience)
        {
            experience = MinimumAmount;

            if (cards == null || cards.Count == MinimumAmount)
                return false;

            foreach (var cardAmount in cards)
            {
                if (cardAmount.Amount <= MinimumAmount ||
                    _cardCatalog.TryGetCard(cardAmount.Id, out var card) is false ||
                    card.ExperienceAmount <= MinimumAmount)
                {
                    return false;
                }

                var availableExperience = int.MaxValue - experience;
                var maximumAmount = availableExperience / card.ExperienceAmount;

                if (cardAmount.Amount > maximumAmount)
                    return false;

                experience += cardAmount.Amount * card.ExperienceAmount;
            }

            return experience > MinimumAmount;
        }

        private void RestoreCards(IEnumerable<ItemAmount> cards)
        {
            foreach (var card in cards)
                _userItems.TryAdd(card.Id, card.Amount);
        }
    }
}