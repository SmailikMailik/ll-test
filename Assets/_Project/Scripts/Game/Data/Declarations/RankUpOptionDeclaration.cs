using System.Collections.Generic;
using LL.Infrastructure.Collections;

namespace LL.Game.Data.Declarations
{
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
}