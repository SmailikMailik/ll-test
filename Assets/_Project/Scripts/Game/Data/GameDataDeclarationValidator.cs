using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Data.Declarations;
using LL.Game.Heroes;
using LL.Game.Identifiers;
using LL.Game.Items;
using LL.Game.Payments;
using LL.Game.RankUp;
using LL.Game.Quests;
using LL.Game.Ranks;
using LL.Game.Rewards;
using LL.Validation;

namespace LL.Game.Data
{
    internal static class GameDataDeclarationValidator
    {
        private const string RankEntriesCode = "rank-catalog.entries.not-empty";
        private const string RankReferenceCode = "rank-up.rank.exists";
        private const string QuestReferenceCode = "rank-up.quest.exists";
        private const string HeroReferenceCode = "rank-up.hero.exists";
        private const string RewardReferenceCode = "rank-up.reward.exists";
        private const string FinalRankCode = "rank-up.rank.not-final";
        private const string RankUpRequiredCode = "rank-up.rank.required";
        private const string RankUpUniqueCode = "rank-up.hero-rank.unique";
        private const string OptionsCode = "rank-up.options.not-empty";
        private const string OptionUniqueCode = "rank-up.option.unique";
        private const string RequirementUniqueCode = "rank-up.requirement.unique";
        private const string RequirementTypeCode = "rank-up.requirement.type.supported";
        private const string RequiredCountCode = "rank-up.quest.count.positive";
        private const string DurationCode = "rank-up.quest.duration.positive";

        internal static void EnsureValid(GameDataDeclaration declaration)
        {
            if (declaration is null)
                throw new ArgumentNullException(nameof(declaration));

            EnsureRanksAreNotEmpty(declaration.Ranks, nameof(declaration));
            var catalogIds = EnsureCatalogIdsAreValid(declaration);
            EnsureRankUpsAreValid(declaration, catalogIds);
        }

        private static void EnsureRanksAreNotEmpty(
            IReadOnlyList<RankDeclaration> ranks,
            string parameterName)
        {
            ValidationRunner.EnsureValid(
                context => ValidationRules.NotEmpty(
                    ranks,
                    context.At(nameof(GameDataDeclaration.Ranks)),
                    RankEntriesCode),
                parameterName);
        }

        private static CatalogIds EnsureCatalogIdsAreValid(GameDataDeclaration declaration)
        {
            var rankIds = EnsureIdsAreValidAndCollect(
                declaration.Ranks,
                entry => new RankId(entry.Id),
                nameof(declaration.Ranks));
            EnsureIdsAreValid(
                declaration.Cards,
                entry => new ItemId(entry.Id),
                nameof(declaration.Cards));
            var heroIds = EnsureIdsAreValidAndCollect(
                declaration.Heroes,
                entry => new HeroId(entry.Id),
                nameof(declaration.Heroes));
            var questIds = EnsureIdsAreValidAndCollect(
                declaration.Quests,
                entry => new QuestId(entry.Id),
                nameof(declaration.Quests));
            var rewardIds = EnsureIdsAreValidAndCollect(
                declaration.Rewards,
                entry => new RewardId(entry.Id),
                nameof(declaration.Rewards));

            return new CatalogIds(rankIds, heroIds, questIds, rewardIds);
        }

        private static HashSet<TId> EnsureIdsAreValidAndCollect<TEntry, TId>(
            IEnumerable<TEntry> entries,
            Func<TEntry, TId> getId,
            string collectionName)
            where TId : struct, IIdentifier
        {
            IdentifierCollectionValidator.EnsureValid(entries, getId, collectionName);
            return entries.Select(getId).ToHashSet();
        }

        private static void EnsureIdsAreValid<TEntry, TId>(
            IEnumerable<TEntry> entries,
            Func<TEntry, TId> getId,
            string collectionName)
            where TId : struct, IIdentifier
        {
            IdentifierCollectionValidator.EnsureValid(entries, getId, collectionName);
        }

        private static void EnsureRankUpsAreValid(
            GameDataDeclaration declaration,
            CatalogIds catalogIds)
        {
            ValidationRunner.EnsureValid(
                context => ValidateRankUps(declaration, catalogIds, context),
                nameof(declaration));
        }

        private static void ValidateRankUps(
            GameDataDeclaration declaration,
            CatalogIds catalogIds,
            ValidationContext context)
        {
            var rankUpsContext = context.At(nameof(declaration.RankUps));
            var rankUpKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var finalRankId = new RankId(declaration.Ranks[declaration.Ranks.Count - 1].Id);

            for (var index = 0; index < declaration.RankUps.Count; index++)
            {
                var rankUp = declaration.RankUps[index];
                var rankUpContext = rankUpsContext.At(index);

                if (ValidationRules.NotNull(rankUp, rankUpContext, RankUpRequiredCode) is false)
                    continue;

                ValidateRankUp(rankUp, finalRankId, catalogIds, rankUpKeys, rankUpContext);
            }

            ValidateRequiredRankUps(declaration.Ranks, catalogIds.Heroes, rankUpKeys, rankUpsContext);
        }

        private static void ValidateRankUp(
            RankUpDeclaration rankUp,
            RankId finalRankId,
            CatalogIds catalogIds,
            ISet<string> rankUpKeys,
            ValidationContext context)
        {
            var heroId = new HeroId(rankUp.HeroId);
            var rankId = new RankId(rankUp.RankId);
            var rewardId = new RewardId(rankUp.RewardId);

            ValidationRules.ReferenceExists(
                rankId,
                catalogIds.Ranks,
                context.At(nameof(rankUp.RankId)),
                RankReferenceCode);
            ValidationRules.ReferenceExists(
                heroId,
                catalogIds.Heroes,
                context.At(nameof(rankUp.HeroId)),
                HeroReferenceCode);
            ValidationRules.ReferenceExists(
                rewardId,
                catalogIds.Rewards,
                context.At(nameof(rankUp.RewardId)),
                RewardReferenceCode);
            ValidationRules.NotEqual(
                rankId,
                finalRankId,
                context.At(nameof(rankUp.RankId)),
                FinalRankCode);
            ValidationRules.TryAddUnique(
                $"{heroId.Value}\n{rankId.Value}",
                rankUpKeys,
                context,
                RankUpUniqueCode);

            ValidateRankUpOptions(rankUp, catalogIds.Quests, context);
        }

        private static void ValidateRequiredRankUps(
            IReadOnlyList<RankDeclaration> ranks,
            IEnumerable<HeroId> heroIds,
            ISet<string> rankUpKeys,
            ValidationContext context)
        {
            foreach (var heroId in heroIds)
            {
                for (var index = 0; index + 1 < ranks.Count; index++)
                {
                    var rankId = new RankId(ranks[index].Id);
                    ValidationRules.ReferenceExists(
                        $"{heroId.Value}\n{rankId.Value}",
                        rankUpKeys,
                        context,
                        RankUpRequiredCode);
                }
            }
        }

        private static void ValidateRankUpOptions(
            RankUpDeclaration rankUp,
            ISet<QuestId> questIds,
            ValidationContext context)
        {
            var optionsContext = context.At(nameof(rankUp.Options));

            if (ValidationRules.NotEmpty(rankUp.Options, optionsContext, OptionsCode) is false)
                return;

            var optionIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (var index = 0; index < rankUp.Options.Count; index++)
            {
                var option = rankUp.Options[index];
                var optionContext = optionsContext.At(index);

                if (ValidationRules.NotNull(option, optionContext, OptionsCode) is false)
                    continue;

                var optionId = new RankUpOptionId(option.OptionId);
                IdentifierValidator.Validate(optionId, optionContext.At(nameof(option.OptionId)));
                ValidationRules.TryAddUnique(optionId.Value, optionIds, optionContext, OptionUniqueCode);
                ValidateRankUpRequirements(option, questIds, optionContext);
            }
        }

        private static void ValidateRankUpRequirements(
            RankUpOptionDeclaration option,
            ISet<QuestId> questIds,
            ValidationContext context)
        {
            var requirementIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (var index = 0; index < option.Requirements.Count; index++)
            {
                var requirement = option.Requirements[index];
                var requirementContext = context.At(nameof(option.Requirements)).At(index);

                if (ValidationRules.NotNull(requirement, requirementContext, OptionsCode) is false)
                    continue;

                ValidateRequirementId(requirement.RequirementId, requirementIds, requirementContext);
                ValidateRankUpRequirement(requirement, questIds, requirementContext);
            }
        }

        private static void ValidateRankUpRequirement(
            RankUpRequirementDeclaration requirement,
            ISet<QuestId> questIds,
            ValidationContext context)
        {
            switch (requirement)
            {
                case QuestRankUpRequirementDeclaration quest:
                    ValidationRules.ReferenceExists(
                        new QuestId(quest.QuestId),
                        questIds,
                        context.At(nameof(quest.QuestId)),
                        QuestReferenceCode);
                    ValidationRules.Positive(
                        quest.RequiredCount,
                        context.At(nameof(quest.RequiredCount)),
                        RequiredCountCode);
                    ValidationRules.Positive(
                        quest.DurationMinutes,
                        context.At(nameof(quest.DurationMinutes)),
                        DurationCode);
                    break;
                case PaymentRankUpRequirementDeclaration payment:
                    EnsurePaymentIsValid(payment.Payment);
                    break;
                default:
                    context.Report(
                        ValidationSeverity.Error,
                        RequirementTypeCode,
                        $"Unsupported rank-up requirement type '{requirement.GetType().Name}'.");
                    break;
            }
        }

        private static void EnsurePaymentIsValid(PaymentDeclaration payment)
        {
            if (payment is null)
                throw new ArgumentNullException(nameof(payment));

            _ = new Payment(new ItemId(payment.ItemId), payment.Amount);
        }

        private static void ValidateRequirementId(
            string value,
            ISet<string> usedIds,
            ValidationContext context)
        {
            var requirementId = new RankUpRequirementId(value);
            IdentifierValidator.Validate(requirementId, context.At(nameof(RankUpRequirementDeclaration.RequirementId)));
            ValidationRules.TryAddUnique(value, usedIds, context, RequirementUniqueCode);
        }

        private sealed class CatalogIds
        {
            internal ISet<RankId> Ranks { get; }
            internal ISet<HeroId> Heroes { get; }
            internal ISet<QuestId> Quests { get; }
            internal ISet<RewardId> Rewards { get; }

            internal CatalogIds(
                ISet<RankId> ranks,
                ISet<HeroId> heroes,
                ISet<QuestId> quests,
                ISet<RewardId> rewards)
            {
                Ranks = ranks;
                Heroes = heroes;
                Quests = quests;
                Rewards = rewards;
            }
        }
    }
}