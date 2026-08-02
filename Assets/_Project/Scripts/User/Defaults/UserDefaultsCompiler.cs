using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Cards;
using LL.Game.Heroes;
using LL.Game.Items;
using LL.Game.Ranks;
using LL.Game.RankUp;
using LL.Game.Rewards;
using LL.Infrastructure.Compilation;
using LL.User.Defaults.Declarations;
using LL.User.Snapshots;
using VContainer;

namespace LL.User.Defaults
{
    internal sealed class UserDefaultsCompiler : IDataCompiler<UserDefaultsDeclaration, UserDefaultsSnapshot>
    {
        private readonly UserDefaultsDeclarationValidator _validator;

        [Inject]
        internal UserDefaultsCompiler(
            RankCatalog ranks,
            HeroCatalog heroes,
            CardCatalog cards,
            RankUpCatalog rankUps,
            RewardCatalog rewards)
        {
            _validator = new UserDefaultsDeclarationValidator(ranks, heroes, cards, rankUps, rewards);
        }

        public UserDefaultsSnapshot Compile(UserDefaultsDeclaration declaration)
        {
            _validator.EnsureValid(declaration);

            var identity = ToUserIdentitySnapshot(declaration.Identity);
            var heroSelection = ToUserHeroSelectionSnapshot(declaration.HeroSelection);
            var heroes = ToUserHeroesSnapshot(declaration.Heroes);
            var items = ToUserItemsSnapshot(declaration.Items);

            return new UserDefaultsSnapshot(identity, heroSelection, heroes, items);
        }

        private static UserIdentitySnapshot ToUserIdentitySnapshot(UserIdentityDefaultDeclaration declaration)
        {
            return new UserIdentitySnapshot(declaration.UserId, declaration.RegionCode);
        }

        private static UserHeroSelectionSnapshot ToUserHeroSelectionSnapshot(
            UserHeroSelectionDefaultDeclaration declaration)
        {
            return new UserHeroSelectionSnapshot(new HeroId(declaration.HeroId));
        }

        private static UserHeroesSnapshot ToUserHeroesSnapshot(
            IReadOnlyList<UserHeroDefaultDeclaration> declarations)
        {
            return new UserHeroesSnapshot(declarations.Select(ToUserHeroSnapshot));
        }

        private static UserHeroSnapshot ToUserHeroSnapshot(UserHeroDefaultDeclaration declaration)
        {
            return new UserHeroSnapshot(
                new HeroId(declaration.HeroId),
                new UserProgressSnapshot(new RankId(declaration.RankId), declaration.Experience),
                Array.Empty<UserRankUpAttemptSnapshot>());
        }

        private static UserItemsSnapshot ToUserItemsSnapshot(
            IReadOnlyList<UserItemDefaultDeclaration> declarations)
        {
            return new UserItemsSnapshot(declarations.Select(ToItemAmount));
        }

        private static ItemAmount ToItemAmount(UserItemDefaultDeclaration declaration)
        {
            return new ItemAmount(new ItemId(declaration.Id), declaration.Amount);
        }
    }
}