using LL.Game.Identifiers;
using LL.Game.Payments.Configuration;
using LL.Validation;

namespace LL.Game.Promotions.Configuration
{
    internal sealed class RankPromotionCatalogConfigValidator : IDataValidator<RankPromotionEntry[]>
    {
        private const string EntriesCode = "rank-promotion.entries.required";
        private const string HeroKeyCode = "rank-promotion.quest.hero.not-empty";
        private const string RequiredAmountCode = "rank-promotion.quest.amount.positive";
        private const string DurationCode = "rank-promotion.quest.duration.positive";
        private static readonly IDataValidator<PaymentEntry> _paymentValidator =
            new PaymentEntryValidator();

        public void Validate(
            RankPromotionEntry[] promotions,
            ValidationContext context)
        {
            if (ValidationRules.NotNull(promotions, context, EntriesCode) is false)
                return;

            IdentifierCollectionValidator.Validate(
                promotions,
                promotion => promotion.RankId,
                context);

            for (var index = 0; index < promotions.Length; index++)
            {
                var promotion = promotions[index];

                if (promotion == null)
                    continue;

                var promotionContext = context.At(index);

                IdentifierValidator.Validate(
                    promotion.QuestId,
                    promotionContext.At(nameof(RankPromotionEntry.QuestId)));

                ValidationRules.NotEmpty(
                    promotion.HeroLocalizationKey,
                    promotionContext.At(nameof(RankPromotionEntry.HeroLocalizationKey)),
                    HeroKeyCode);

                ValidationRules.Positive(
                    promotion.RequiredAmount,
                    promotionContext.At(nameof(RankPromotionEntry.RequiredAmount)),
                    RequiredAmountCode);

                ValidationRules.Positive(
                    promotion.DurationMinutes,
                    promotionContext.At(nameof(RankPromotionEntry.DurationMinutes)),
                    DurationCode);

                _paymentValidator.Validate(
                    promotion.QuestPayment,
                    promotionContext.At(nameof(RankPromotionEntry.QuestPayment)));
                _paymentValidator.Validate(
                    promotion.InstantPayment,
                    promotionContext.At(nameof(RankPromotionEntry.InstantPayment)));

                IdentifierValidator.Validate(
                    promotion.RewardId,
                    promotionContext.At(nameof(RankPromotionEntry.RewardId)));
            }
        }
    }
}