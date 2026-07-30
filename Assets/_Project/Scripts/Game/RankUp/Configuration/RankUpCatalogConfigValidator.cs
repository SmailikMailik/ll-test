using LL.Game.Identifiers;
using LL.Game.Payments.Configuration;
using LL.Validation;

namespace LL.Game.RankUp.Configuration
{
    internal sealed class RankUpCatalogConfigValidator : IDataValidator<RankUpEntry[]>
    {
        private const string EntriesCode = "rank-up.entries.required";
        private const string HeroKeyCode = "rank-up.quest.hero.not-empty";
        private const string RequiredAmountCode = "rank-up.quest.amount.positive";
        private const string DurationCode = "rank-up.quest.duration.positive";
        private static readonly IDataValidator<PaymentEntry> _paymentValidator = new PaymentEntryValidator();

        public void Validate(
            RankUpEntry[] rankUps,
            ValidationContext context)
        {
            if (ValidationRules.NotNull(rankUps, context, EntriesCode) is false)
                return;

            IdentifierCollectionValidator.Validate(
                rankUps,
                rankUp => rankUp.RankId,
                context);

            for (var index = 0; index < rankUps.Length; index++)
            {
                var rankUp = rankUps[index];

                if (rankUp == null)
                    continue;

                var rankUpContext = context.At(index);

                IdentifierValidator.Validate(
                    rankUp.QuestId,
                    rankUpContext.At(nameof(RankUpEntry.QuestId)));

                ValidationRules.NotEmpty(
                    rankUp.HeroLocalizationKey,
                    rankUpContext.At(nameof(RankUpEntry.HeroLocalizationKey)),
                    HeroKeyCode);

                ValidationRules.Positive(
                    rankUp.RequiredAmount,
                    rankUpContext.At(nameof(RankUpEntry.RequiredAmount)),
                    RequiredAmountCode);

                ValidationRules.Positive(
                    rankUp.DurationMinutes,
                    rankUpContext.At(nameof(RankUpEntry.DurationMinutes)),
                    DurationCode);

                _paymentValidator.Validate(
                    rankUp.QuestPayment,
                    rankUpContext.At(nameof(RankUpEntry.QuestPayment)));
                _paymentValidator.Validate(
                    rankUp.InstantPayment,
                    rankUpContext.At(nameof(RankUpEntry.InstantPayment)));

                IdentifierValidator.Validate(
                    rankUp.RewardId,
                    rankUpContext.At(nameof(RankUpEntry.RewardId)));
            }
        }
    }
}