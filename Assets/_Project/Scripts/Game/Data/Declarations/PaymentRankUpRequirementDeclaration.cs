using System;

namespace LL.Game.Data.Declarations
{
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