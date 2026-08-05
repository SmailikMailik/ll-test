using System;
using LL.Game.Heroes;
using LL.Game.Payments.Services;
using LL.Game.Ranks;
using VContainer;

namespace LL.Game.RankUp.Services
{
    internal sealed class PaymentRankUpRequirementService : IRankUpRequirementKindService
    {
        private readonly IPaymentService _paymentService;

        [Inject]
        internal PaymentRankUpRequirementService(IPaymentService paymentService)
        {
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        }

        public bool Supports(RankUpRequirementDefinition requirement)
        {
            return requirement is PaymentRankUpRequirementDefinition;
        }

        public bool IsSatisfied(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            RankUpRequirementDefinition requirement)
        {
            return requirement is PaymentRankUpRequirementDefinition payment &&
                   _paymentService.CanPay(payment.Payment);
        }

        public bool TryActivate(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            RankUpRequirementDefinition requirement)
        {
            return Supports(requirement);
        }

        public bool TryCommit(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            RankUpRequirementDefinition requirement)
        {
            return requirement is PaymentRankUpRequirementDefinition payment &&
                   _paymentService.TryPay(payment.Payment);
        }

        public bool TryRollback(RankUpRequirementDefinition requirement)
        {
            return requirement is PaymentRankUpRequirementDefinition payment &&
                   _paymentService.TryRefund(payment.Payment);
        }
    }
}