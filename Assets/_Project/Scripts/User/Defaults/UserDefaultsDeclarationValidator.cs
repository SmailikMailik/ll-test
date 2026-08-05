using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Cards;
using LL.Game.Heroes;
using LL.Game.Identifiers;
using LL.Game.Items;
using LL.Game.Ranks;
using LL.Game.RankUp;
using LL.Game.Rewards;
using LL.User.Defaults.Declarations;
using LL.Validation;

namespace LL.User.Defaults
{
    internal sealed class UserDefaultsDeclarationValidator
    {
        private const string HeroExistsCode = "user-defaults.hero-selection.hero.exists";
        private const string SelectedHeroDefaultCode = "user-defaults.hero-selection.default.exists";
        private const string RankExistsCode = "user-defaults.progress.rank.exists";
        private const string ExperienceCode = "user-defaults.progress.experience.non-negative";
        private const string ExperienceMaxCode = "user-defaults.progress.experience.maximum";
        private const string FinalRankExperienceCode = "user-defaults.progress.final-rank-experience.zero";
        private const string BuiltInItemCode = "user-defaults.item.built-in.exists";
        private const string CardItemCode = "card.user-item.exists";
        private const string RankUpPaymentItemCode = "rank-up.payment.user-item.exists";
        private const string RewardItemCode = "reward.user-item.exists";

        private readonly RankCatalog _ranks;
        private readonly CardCatalog _cards;
        private readonly RankUpCatalog _rankUps;
        private readonly RewardCatalog _rewards;
        private readonly ISet<HeroId> _availableHeroIds;
        private readonly ISet<RankId> _availableRankIds;

        internal UserDefaultsDeclarationValidator(
            RankCatalog ranks,
            HeroCatalog heroes,
            CardCatalog cards,
            RankUpCatalog rankUps,
            RewardCatalog rewards)
        {
            _ranks = ranks ?? throw new ArgumentNullException(nameof(ranks));

            if (heroes is null)
                throw new ArgumentNullException(nameof(heroes));

            _cards = cards ?? throw new ArgumentNullException(nameof(cards));
            _rankUps = rankUps ?? throw new ArgumentNullException(nameof(rankUps));
            _rewards = rewards ?? throw new ArgumentNullException(nameof(rewards));
            _availableHeroIds = heroes.Heroes.Select(hero => hero.Id).ToHashSet();
            _availableRankIds = _ranks.Ranks.Select(rank => rank.Id).ToHashSet();
        }

        internal void EnsureValid(UserDefaultsDeclaration declaration)
        {
            if (declaration is null)
                throw new ArgumentNullException(nameof(declaration));

            var selectedHeroId = new HeroId(declaration.HeroSelection.HeroId);

            EnsureHeroDeclarationsAreValid(declaration.Heroes);
            EnsureSelectedHeroExists(selectedHeroId, declaration.Heroes);
            EnsureItemDeclarationsAreValid(declaration.Items);
            EnsureRequiredItemsExist(declaration.Items);
        }

        private void EnsureHeroDeclarationsAreValid(IReadOnlyList<UserHeroDefaultDeclaration> declarations)
        {
            if (declarations is null)
                throw new ArgumentNullException(nameof(declarations));

            IdentifierCollectionValidator.EnsureValid(
                declarations,
                declaration => new HeroId(declaration.HeroId),
                nameof(declarations));

            ValidationRunner.EnsureValid(
                context => ValidateHeroes(declarations, context),
                nameof(declarations));
        }

        private void ValidateHeroes(
            IReadOnlyList<UserHeroDefaultDeclaration> declarations,
            ValidationContext context)
        {
            for (var index = 0; index < declarations.Count; index++)
                ValidateHero(declarations[index], context.At(index));
        }

        private void ValidateHero(UserHeroDefaultDeclaration declaration, ValidationContext context)
        {
            ValidationRules.ReferenceExists(
                new HeroId(declaration.HeroId),
                _availableHeroIds,
                context.At(nameof(declaration.HeroId)),
                HeroExistsCode);
            ValidateProgress(
                new RankId(declaration.RankId),
                declaration.Experience,
                context);
        }

        private static void EnsureSelectedHeroExists(
            HeroId selectedHeroId,
            IEnumerable<UserHeroDefaultDeclaration> heroes)
        {
            var heroIds = heroes.Select(hero => new HeroId(hero.HeroId)).ToHashSet();

            ValidationRunner.EnsureValid(
                context => ValidationRules.ReferenceExists(
                    selectedHeroId,
                    heroIds,
                    context.At(nameof(UserDefaultsDeclaration.HeroSelection)),
                    SelectedHeroDefaultCode),
                nameof(selectedHeroId));
        }

        private static void EnsureItemDeclarationsAreValid(IReadOnlyList<UserItemDefaultDeclaration> declarations)
        {
            if (declarations is null)
                throw new ArgumentNullException(nameof(declarations));

            IdentifierCollectionValidator.EnsureValid(
                declarations,
                declaration => new ItemId(declaration.Id),
                nameof(declarations));

            foreach (var declaration in declarations)
                _ = new ItemAmount(new ItemId(declaration.Id), declaration.Amount);
        }

        private void EnsureRequiredItemsExist(IEnumerable<UserItemDefaultDeclaration> declarations)
        {
            var itemIds = declarations.Select(declaration => new ItemId(declaration.Id)).ToHashSet();

            ValidationRunner.EnsureValid(
                context => ValidateRequiredItems(itemIds, context),
                nameof(declarations));
        }

        private void ValidateRequiredItems(ISet<ItemId> itemIds, ValidationContext context)
        {
            ValidateBuiltInItems(itemIds, context);
            ValidateCardItems(itemIds, context);
            ValidateRankUpPaymentItems(itemIds, context);
            ValidateRewardItems(itemIds, context);
        }

        private void ValidateProgress(
            RankId rankId,
            int experience,
            ValidationContext context)
        {
            if (ValidationRules.ReferenceExists(
                    rankId,
                    _availableRankIds,
                    context.At(nameof(UserHeroDefaultDeclaration.RankId)),
                    RankExistsCode) is false)
            {
                return;
            }

            var rank = _ranks.GetRank(rankId);
            var experienceContext = context.At(nameof(UserHeroDefaultDeclaration.Experience));

            if (ValidationRules.NonNegative(experience, experienceContext, ExperienceCode) is false)
                return;

            var rankIndex = rank.Number - 1;

            if (rankIndex + 1 < _ranks.Ranks.Count)
            {
                ValidationRules.LessThanOrEqual(
                    experience,
                    _ranks.Ranks[rankIndex + 1].RequiredExperience,
                    experienceContext,
                    ExperienceMaxCode);
                return;
            }

            ValidationRules.Equal(
                experience,
                0,
                experienceContext,
                FinalRankExperienceCode);
        }

        private static void ValidateBuiltInItems(ISet<ItemId> itemIds, ValidationContext context)
        {
            foreach (var id in ItemIds.All)
                ValidationRules.ReferenceExists(id, itemIds, context, BuiltInItemCode);
        }

        private void ValidateCardItems(ISet<ItemId> itemIds, ValidationContext context)
        {
            foreach (var card in _cards.Cards)
                ValidationRules.ReferenceExists(card.Id, itemIds, context, CardItemCode);
        }

        private void ValidateRankUpPaymentItems(ISet<ItemId> itemIds, ValidationContext context)
        {
            foreach (var rankUp in _rankUps.Definitions)
            {
                foreach (var payment in rankUp.Options
                             .SelectMany(option => option.Requirements)
                             .OfType<PaymentRankUpRequirementDefinition>())
                {
                    ValidationRules.ReferenceExists(
                        payment.Payment.ItemId,
                        itemIds,
                        context,
                        RankUpPaymentItemCode);
                }
            }
        }

        private void ValidateRewardItems(ISet<ItemId> itemIds, ValidationContext context)
        {
            foreach (var reward in _rewards.Rewards)
            {
                foreach (var item in reward.Items)
                    ValidationRules.ReferenceExists(item.Id, itemIds, context, RewardItemCode);
            }
        }
    }
}