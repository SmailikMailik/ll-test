using LL.Game.Identifiers;
using LL.Game.Payments.Configuration;
using LL.Validation;

namespace LL.Game.Promotions.Configuration
{
    internal sealed class RankPromotionCatalogConfigValidator : IDataValidator<RankPromotionEntry[]>
    {
        private const string PromotionsCode = "rank-promotion.entries.required";
        private const string DurationCode = "rank-promotion.duration.positive";
        private const string RequiredAmountCode = "rank-promotion.requirement.amount.positive";
        private const string TitleKeyCode = "rank-promotion.localization.title.not-empty";
        private const string DescriptionKeyCode = "rank-promotion.localization.description.not-empty";
        private const string TargetKeyCode = "rank-promotion.localization.target.not-empty";
        private static readonly IDataValidator<PaymentEntry> _paymentValidator =
            new PaymentEntryValidator();

        public void Validate(
            RankPromotionEntry[] promotions,
            ValidationContext context)
        {
            if (ValidationRules.NotNull(promotions, context, PromotionsCode) is false)
                return;

            RankPromotionCatalogValidator.Validate(
                promotions,
                promotion => promotion.Rank,
                context);

            for (var index = 0; index < promotions.Length; index++)
            {
                var promotion = promotions[index];

                if (promotion == null)
                    continue;

                var promotionContext = context.At(index);

                ValidationRules.Positive(
                    promotion.DurationMinutes,
                    promotionContext.At(nameof(RankPromotionEntry.DurationMinutes)),
                    DurationCode);

                IdentifierValidator.Validate(
                    promotion.RequirementId,
                    promotionContext.At(nameof(RankPromotionEntry.RequirementId)));

                ValidationRules.Positive(
                    promotion.RequiredAmount,
                    promotionContext.At(nameof(RankPromotionEntry.RequiredAmount)),
                    RequiredAmountCode);

                ValidationRules.NotEmpty(
                    promotion.TitleLocalizationKey,
                    promotionContext.At(nameof(RankPromotionEntry.TitleLocalizationKey)),
                    TitleKeyCode);

                ValidationRules.NotEmpty(
                    promotion.DescriptionLocalizationKey,
                    promotionContext.At(nameof(RankPromotionEntry.DescriptionLocalizationKey)),
                    DescriptionKeyCode);

                ValidationRules.NotEmpty(
                    promotion.TargetLocalizationKey,
                    promotionContext.At(nameof(RankPromotionEntry.TargetLocalizationKey)),
                    TargetKeyCode);

                _paymentValidator.Validate(
                    promotion.OrderPayment,
                    promotionContext.At(nameof(RankPromotionEntry.OrderPayment)));
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