using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Cards;
using LL.Game.Heroes;
using LL.Game.Identifiers;
using LL.Game.Items;
using LL.Game.RankUp;
using LL.Game.Ranks;
using LL.Game.Rewards;
using LL.Infrastructure.Compilation;
using LL.User.Defaults.Declarations;
using LL.User.Snapshots;
using LL.Validation;
using VContainer;

namespace LL.User.Defaults
{
    internal sealed class UserDefaultsCompiler : IDataCompiler<UserDefaultsDeclaration, UserDefaultsSnapshot>
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
        private readonly HeroCatalog _heroes;
        private readonly CardCatalog _cards;
        private readonly RankUpCatalog _rankUps;
        private readonly RewardCatalog _rewards;

        [Inject]
        internal UserDefaultsCompiler(
            RankCatalog ranks,
            HeroCatalog heroes,
            CardCatalog cards,
            RankUpCatalog rankUps,
            RewardCatalog rewards)
        {
            _ranks = ranks ?? throw new ArgumentNullException(nameof(ranks));
            _heroes = heroes ?? throw new ArgumentNullException(nameof(heroes));
            _cards = cards ?? throw new ArgumentNullException(nameof(cards));
            _rankUps = rankUps ?? throw new ArgumentNullException(nameof(rankUps));
            _rewards = rewards ?? throw new ArgumentNullException(nameof(rewards));
        }

        public UserDefaultsSnapshot Compile(UserDefaultsDeclaration declaration)
        {
            if (declaration is null)
                throw new ArgumentNullException(nameof(declaration));

            var heroId = new HeroId(declaration.HeroSelection.HeroId);
            var heroes = CompileHeroes(declaration.Heroes, _heroes, _ranks);
            EnsureSelectedHeroExists(heroId, heroes);

            var itemAmounts = CompileItemAmounts(declaration.Items);
            EnsureRequiredItemsExist(itemAmounts, _cards, _rankUps, _rewards);

            var identity = new UserIdentitySnapshot(
                declaration.Identity.UserId,
                declaration.Identity.RegionCode);
            var heroSelection = new UserHeroSelectionSnapshot(heroId);
            var items = new UserItemsSnapshot(itemAmounts);

            return new UserDefaultsSnapshot(identity, heroSelection, heroes, items);
        }

        private static UserHeroesSnapshot CompileHeroes(
            IReadOnlyList<UserHeroDefaultDeclaration> declarations,
            HeroCatalog heroes,
            RankCatalog ranks)
        {
            if (declarations is null)
                throw new ArgumentNullException(nameof(declarations));

            IdentifierCollectionValidator.EnsureValid(
                declarations,
                declaration => new HeroId(declaration.HeroId),
                nameof(declarations));

            var availableHeroIds = heroes.Heroes.Select(hero => hero.Id).ToHashSet();

            ValidationRunner.EnsureValid(
                context =>
                {
                    for (var index = 0; index < declarations.Count; index++)
                    {
                        var declaration = declarations[index];
                        var heroContext = context.At(index);
                        ValidationRules.ReferenceExists(
                            new HeroId(declaration.HeroId),
                            availableHeroIds,
                            heroContext.At(nameof(declaration.HeroId)),
                            HeroExistsCode);
                        EnsureProgressIsValid(
                            new RankId(declaration.RankId),
                            declaration.Experience,
                            ranks,
                            heroContext);
                    }
                },
                nameof(declarations));

            return new UserHeroesSnapshot(declarations.Select(ToUserHeroSnapshot));
        }

        private static UserHeroSnapshot ToUserHeroSnapshot(UserHeroDefaultDeclaration declaration)
        {
            return new UserHeroSnapshot(
                new HeroId(declaration.HeroId),
                new UserProgressSnapshot(new RankId(declaration.RankId), declaration.Experience),
                Array.Empty<UserRankUpAttemptSnapshot>());
        }

        private static void EnsureSelectedHeroExists(
            HeroId heroId,
            UserHeroesSnapshot heroes)
        {
            var heroIds = heroes.Heroes.Select(hero => hero.HeroId).ToHashSet();

            ValidationRunner.EnsureValid(
                context => ValidationRules.ReferenceExists(
                    heroId,
                    heroIds,
                    context.At(nameof(UserDefaultsDeclaration.HeroSelection)),
                    SelectedHeroDefaultCode),
                nameof(heroId));
        }

        private static IReadOnlyList<ItemAmount> CompileItemAmounts(
            IReadOnlyList<UserItemDefaultDeclaration> declarations)
        {
            if (declarations is null)
                throw new ArgumentNullException(nameof(declarations));

            IdentifierCollectionValidator.EnsureValid(
                declarations,
                declaration => new ItemId(declaration.Id),
                nameof(declarations));

            var amounts = new ItemAmount[declarations.Count];

            for (var index = 0; index < declarations.Count; index++)
            {
                var declaration = declarations[index];
                amounts[index] = new ItemAmount(new ItemId(declaration.Id), declaration.Amount);
            }

            return Array.AsReadOnly(amounts);
        }

        private static void EnsureProgressIsValid(
            RankId rankId,
            int experience,
            RankCatalog ranks,
            ValidationContext context)
        {
            var rankIds = ranks.Ranks.Select(definition => definition.Id).ToHashSet();
            ranks.TryGetRank(rankId, out var rank);

            if (ValidationRules.ReferenceExists(
                    rankId,
                    rankIds,
                    context.At(nameof(UserHeroDefaultDeclaration.RankId)),
                    RankExistsCode) is false)
            {
                return;
            }

            var experienceContext = context.At(nameof(UserHeroDefaultDeclaration.Experience));

            if (ValidationRules.NonNegative(experience, experienceContext, ExperienceCode) is false)
                return;

            var rankIndex = rank.Number - 1;

            if (rankIndex + 1 < ranks.Ranks.Count)
            {
                ValidationRules.LessThanOrEqual(
                    experience,
                    ranks.Ranks[rankIndex + 1].RequiredExperience,
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

        private static void EnsureRequiredItemsExist(
            IReadOnlyList<ItemAmount> amounts,
            CardCatalog cards,
            RankUpCatalog rankUps,
            RewardCatalog rewards)
        {
            var itemIds = amounts.Select(amount => amount.Id).ToHashSet();

            ValidationRunner.EnsureValid(
                context =>
                {
                    ValidateBuiltInItems(itemIds, context);
                    ValidateCardItems(itemIds, cards, context);
                    ValidateRankUpPaymentItems(itemIds, rankUps, context);
                    ValidateRewardItems(itemIds, rewards, context);
                },
                nameof(amounts));
        }

        private static void ValidateBuiltInItems(
            ISet<ItemId> itemIds,
            ValidationContext context)
        {
            foreach (var id in ItemIds.All)
                ValidationRules.ReferenceExists(id, itemIds, context, BuiltInItemCode);
        }

        private static void ValidateCardItems(
            ISet<ItemId> itemIds,
            CardCatalog cards,
            ValidationContext context)
        {
            foreach (var card in cards.Cards)
                ValidationRules.ReferenceExists(card.Id, itemIds, context, CardItemCode);
        }

        private static void ValidateRankUpPaymentItems(
            ISet<ItemId> itemIds,
            RankUpCatalog rankUps,
            ValidationContext context)
        {
            foreach (var rankUp in rankUps.Definitions)
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

        private static void ValidateRewardItems(
            ISet<ItemId> itemIds,
            RewardCatalog rewards,
            ValidationContext context)
        {
            foreach (var reward in rewards.Rewards)
            {
                foreach (var item in reward.Items)
                    ValidationRules.ReferenceExists(item.Id, itemIds, context, RewardItemCode);
            }
        }
    }
}