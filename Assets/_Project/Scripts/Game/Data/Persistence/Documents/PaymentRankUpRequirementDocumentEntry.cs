using System;

namespace LL.Game.Data.Persistence.Documents
{
    internal sealed class PaymentRankUpRequirementDocumentEntry
    {
        public string RequirementId { get; }
        public PaymentDocumentEntry Payment { get; }

        public PaymentRankUpRequirementDocumentEntry(
            string requirementId,
            PaymentDocumentEntry payment)
        {
            RequirementId = requirementId;
            Payment = payment ?? throw new ArgumentNullException(nameof(payment));
        }
    }
}