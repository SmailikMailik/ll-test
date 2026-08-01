using System;

namespace LL.Game.Data.Declarations
{
    internal sealed class PaymentRankUpRequirementDeclaration : RankUpRequirementDeclaration
    {
        internal PaymentDeclaration Payment { get; }

        internal PaymentRankUpRequirementDeclaration(
            string requirementId,
            PaymentDeclaration payment) : base(requirementId)
        {
            Payment = payment ?? throw new ArgumentNullException(nameof(payment));
        }
    }
}