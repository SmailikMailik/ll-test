using System;

namespace LL.Game.Data.Persistence.Documents
{
    internal sealed class PaymentRankUpRequirementDocumentEntry : RankUpRequirementDocumentEntry
    {
        public PaymentDocumentEntry Payment { get; }

        public PaymentRankUpRequirementDocumentEntry(
            string requirementId,
            PaymentDocumentEntry payment) : base(requirementId)
        {
            Payment = payment ?? throw new ArgumentNullException(nameof(payment));
        }
    }
}