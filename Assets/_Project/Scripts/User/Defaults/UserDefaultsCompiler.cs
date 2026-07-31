using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Cards;
using LL.Game.Items;
using LL.Game.RankUp;
using LL.Game.Ranks;
using LL.Game.Rewards;
using LL.User.Defaults.Declarations;
using LL.User.Snapshots;
using VContainer;

namespace LL.User.Defaults
{
    internal sealed class UserDefaultsCompiler
    {
        [Inject]
        internal UserDefaultsCompiler() { }

        internal UserDefaultsSnapshot Compile(
            UserDefaultsDeclaration declaration,
            RankCatalog ranks,
            CardCatalog cards,
            RankUpCatalog rankUps,
            RewardCatalog rewards)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            if (ranks == null)
                throw new ArgumentNullException(nameof(ranks));

            if (cards == null)
                throw new ArgumentNullException(nameof(cards));

            if (rankUps == null)
                throw new ArgumentNullException(nameof(rankUps));

            if (rewards == null)
                throw new ArgumentNullException(nameof(rewards));

            var rankId = new RankId(declaration.RankId);
            EnsureProgressIsValid(rankId, declaration.Experience, ranks);

            var itemAmounts = CompileItems(declaration.Items);
            EnsureRequiredItemsExist(itemAmounts, cards, rankUps, rewards);

            return new UserDefaultsSnapshot(
                new UserIdentitySnapshot(declaration.UserId, declaration.RegionCode),
                new UserItemsSnapshot(itemAmounts),
                new UserProgressSnapshot(rankId, declaration.Experience));
        }

        private static IReadOnlyList<ItemAmount> CompileItems(
            IReadOnlyList<UserItemDefaultsDeclaration> declarations)
        {
            if (declarations == null)
                throw new ArgumentNullException(nameof(declarations));

            var amounts = new ItemAmount[declarations.Count];

            for (var index = 0; index < declarations.Count; index++)
            {
                var declaration = declarations[index];

                if (declaration == null)
                    throw new ArgumentException("User defaults cannot contain null item entries.", nameof(declarations));

                amounts[index] = new ItemAmount(new ItemId(declaration.Id), declaration.Amount);
            }

            return Array.AsReadOnly(amounts);
        }

        private static void EnsureProgressIsValid(
            RankId rankId,
            int experience,
            RankCatalog ranks)
        {
            if (ranks.TryGetRank(rankId, out _) is false)
                throw new ArgumentException($"User defaults reference unknown rank ID '{rankId}'.", nameof(rankId));

            var rankIndex = FindRankIndex(ranks.Ranks, rankId);
            var nextRank = rankIndex + 1 < ranks.Ranks.Count
                ? ranks.Ranks[rankIndex + 1]
                : null;

            if (nextRank == null && experience != 0)
            {
                throw new ArgumentException(
                    $"Experience at final rank '{rankId}' must be zero.",
                    nameof(experience));
            }

            if (nextRank != null && experience > nextRank.RequiredExperience)
            {
                throw new ArgumentException(
                    $"Experience at rank '{rankId}' must not exceed {nextRank.RequiredExperience}.",
                    nameof(experience));
            }
        }

        private static int FindRankIndex(
            IReadOnlyList<RankDefinition> ranks,
            RankId rankId)
        {
            for (var index = 0; index < ranks.Count; index++)
            {
                if (ranks[index].Id.Equals(rankId))
                    return index;
            }

            throw new ArgumentException($"User defaults reference unknown rank ID '{rankId}'.", nameof(rankId));
        }

        private static void EnsureRequiredItemsExist(
            IReadOnlyList<ItemAmount> amounts,
            CardCatalog cards,
            RankUpCatalog rankUps,
            RewardCatalog rewards)
        {
            var itemIds = amounts.Select(amount => amount.Id).ToHashSet();

            foreach (var id in ItemIds.All)
                EnsureItemExists(itemIds, id);

            foreach (var card in cards.Cards)
                EnsureItemExists(itemIds, card.Id);

            foreach (var rankUp in rankUps.Definitions)
            {
                EnsureItemExists(itemIds, rankUp.Quest.Payment.ItemId);
                EnsureItemExists(itemIds, rankUp.InstantPayment.ItemId);
            }

            foreach (var reward in rewards.Rewards)
            {
                foreach (var item in reward.Items)
                    EnsureItemExists(itemIds, item.Id);
            }
        }

        private static void EnsureItemExists(
            ISet<ItemId> itemIds,
            ItemId requiredId)
        {
            if (itemIds.Contains(requiredId) is false)
            {
                throw new ArgumentException(
                    $"User defaults do not contain required item ID '{requiredId}'.",
                    nameof(itemIds));
            }
        }
    }
}