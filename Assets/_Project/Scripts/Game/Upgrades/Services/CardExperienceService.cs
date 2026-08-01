using System;
using System.Collections.Generic;
using LL.Game.Cards;
using LL.Game.Heroes;
using LL.Game.Items;
using LL.Game.Ranks.Services;
using LL.User.State;
using LL.User.State.Items;
using VContainer;

namespace LL.Game.Upgrades.Services
{
    internal sealed class CardExperienceService : ICardExperienceService
    {
        private const int MinAmount = 0;

        private readonly IUserItemsCommands _userItems;
        private readonly IHeroExperienceService _experienceService;
        private readonly CardCatalog _cardCatalog;
        private readonly IUserStateChangeBatch _changeBatch;

        [Inject]
        internal CardExperienceService(
            IUserItemsCommands userItems,
            IHeroExperienceService experienceService,
            CardCatalog cardCatalog,
            IUserStateChangeBatch changeBatch)
        {
            _userItems = userItems ?? throw new ArgumentNullException(nameof(userItems));
            _experienceService = experienceService ?? throw new ArgumentNullException(nameof(experienceService));
            _cardCatalog = cardCatalog ?? throw new ArgumentNullException(nameof(cardCatalog));
            _changeBatch = changeBatch ?? throw new ArgumentNullException(nameof(changeBatch));
        }

        public bool TryApply(HeroId heroId, IReadOnlyList<ItemAmount> cards)
        {
            return _changeBatch.Execute(() => TryApplyCore(heroId, cards));
        }

        private bool TryApplyCore(HeroId heroId, IReadOnlyList<ItemAmount> cards)
        {
            if (TryGetApplication(heroId, cards, out var application) is false)
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

            if (_experienceService.TryGrant(heroId, application.GrantedExperience))
                return true;

            RestoreCards(spentCards);
            return false;
        }

        public bool TryGetApplication(
            HeroId heroId,
            IReadOnlyList<ItemAmount> cards,
            out ExperienceApplication application)
        {
            application = default;

            if (TryCalculateExperience(cards, out var grantedExperience) is false)
                return false;

            var appliedExperience = _experienceService.GetApplicableExperience(heroId, grantedExperience);

            if (appliedExperience <= MinAmount)
                return false;

            application = new ExperienceApplication(grantedExperience, appliedExperience);
            return true;
        }

        private bool TryCalculateExperience(IReadOnlyList<ItemAmount> cards, out int experience)
        {
            experience = MinAmount;

            if (cards is null || cards.Count == MinAmount)
                return false;

            foreach (var cardAmount in cards)
            {
                if (cardAmount.Amount <= MinAmount ||
                    _cardCatalog.TryGetCard(cardAmount.Id, out var card) is false ||
                    card.ExperienceAmount <= MinAmount)
                {
                    return false;
                }

                var availableExperience = int.MaxValue - experience;
                var maxAmount = availableExperience / card.ExperienceAmount;

                if (cardAmount.Amount > maxAmount)
                    return false;

                experience += cardAmount.Amount * card.ExperienceAmount;
            }

            return experience > MinAmount;
        }

        private void RestoreCards(IEnumerable<ItemAmount> cards)
        {
            foreach (var card in cards)
                _userItems.TryAdd(card.Id, card.Amount);
        }
    }
}