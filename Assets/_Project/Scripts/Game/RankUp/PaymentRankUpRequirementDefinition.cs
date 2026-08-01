using LL.Game.Payments;

namespace LL.Game.RankUp
{
    internal sealed class PaymentRankUpRequirementDefinition : RankUpRequirementDefinition
    {
        internal Payment Payment { get; }

        internal PaymentRankUpRequirementDefinition(
            RankUpRequirementId id,
            Payment payment)
            : base(id)
        {
            Payment = payment;
        }
    }
}