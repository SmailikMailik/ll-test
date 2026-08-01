using System;
using System.Collections.Generic;
using LL.Infrastructure.Collections;

namespace LL.Game.Data.Declarations
{
    internal sealed class RankUpDeclaration
    {
        internal string HeroId { get; }
        internal string RankId { get; }
        internal string RewardId { get; }
        internal IReadOnlyList<RankUpOptionDeclaration> Options { get; }

        internal RankUpDeclaration(
            string heroId,
            string rankId,
            string rewardId,
            IEnumerable<RankUpOptionDeclaration> options)
        {
            HeroId = heroId;
            RankId = rankId;
            RewardId = rewardId;
            Options = options.ToReadOnlyCopy();
        }
    }

    internal sealed class RankUpOptionDeclaration
    {
        internal string OptionId { get; }
        internal IReadOnlyList<QuestRankUpRequirementDeclaration> Quests { get; }
        internal IReadOnlyList<PaymentRankUpRequirementDeclaration> Payments { get; }

        internal RankUpOptionDeclaration(
            string optionId,
            IEnumerable<QuestRankUpRequirementDeclaration> quests,
            IEnumerable<PaymentRankUpRequirementDeclaration> payments)
        {
            OptionId = optionId;
            Quests = quests.ToReadOnlyCopy();
            Payments = payments.ToReadOnlyCopy();
        }
    }

    internal sealed class QuestRankUpRequirementDeclaration
    {
        internal string RequirementId { get; }
        internal string QuestId { get; }
        internal int RequiredCount { get; }
        internal int DurationMinutes { get; }

        internal QuestRankUpRequirementDeclaration(
            string requirementId,
            string questId,
            int requiredCount,
            int durationMinutes)
        {
            RequirementId = requirementId;
            QuestId = questId;
            RequiredCount = requiredCount;
            DurationMinutes = durationMinutes;
        }
    }

    internal sealed class PaymentRankUpRequirementDeclaration
    {
        internal string RequirementId { get; }
        internal PaymentDeclaration Payment { get; }

        internal PaymentRankUpRequirementDeclaration(
            string requirementId,
            PaymentDeclaration payment)
        {
            RequirementId = requirementId;
            Payment = payment ?? throw new ArgumentNullException(nameof(payment));
        }
    }
}