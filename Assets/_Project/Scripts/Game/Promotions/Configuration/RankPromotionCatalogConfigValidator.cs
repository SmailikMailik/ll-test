using LL.Game.Data.Validation;
using LL.Game.Identifiers;
using LL.Game.Promotions;

namespace LL.Game.Promotions.Configuration
{
    internal sealed class RankPromotionCatalogConfigValidator : IDataValidator<RankPromotionEntry[]>
    {
        private const string DurationCode = "rank-promotion.duration.positive";
        private const string RequiredAmountCode = "rank-promotion.requirement.amount.positive";
        private const string TitleKeyCode = "rank-promotion.localization.title.not-empty";
        private const string DescriptionKeyCode = "rank-promotion.localization.description.not-empty";
        private const string TargetKeyCode = "rank-promotion.localization.target.not-empty";
        private const string SoftPriceCode = "rank-promotion.soft-price.positive";
        private const string HardPriceCode = "rank-promotion.hard-price.positive";

        public void Validate(
            RankPromotionEntry[] promotions,
            ValidationContext context)
        {
            RankPromotionCatalogValidator.Validate(
                promotions,
                promotion => promotion.Rank,
                context);

            if (promotions == null)
                return;

            for (var index = 0; index < promotions.Length; index++)
            {
                var promotion = promotions[index];

                if (promotion == null)
                    continue;

                var promotionContext = context.At(index);

                ValidationRules.Positive(
                    promotion.DurationMinutes,
                    promotionContext.At("DurationMinutes"),
                    DurationCode);

                IdentifierValidator.Validate(
                    promotion.RequirementId,
                    promotionContext.At("RequirementId"));

                ValidationRules.Positive(
                    promotion.RequiredAmount,
                    promotionContext.At("RequiredAmount"),
                    RequiredAmountCode);

                ValidationRules.NotEmpty(
                    promotion.TitleLocalizationKey,
                    promotionContext.At("TitleLocalizationKey"),
                    TitleKeyCode);

                ValidationRules.NotEmpty(
                    promotion.DescriptionLocalizationKey,
                    promotionContext.At("DescriptionLocalizationKey"),
                    DescriptionKeyCode);

                ValidationRules.NotEmpty(
                    promotion.TargetLocalizationKey,
                    promotionContext.At("TargetLocalizationKey"),
                    TargetKeyCode);

                ValidationRules.Positive(
                    promotion.SoftPrice,
                    promotionContext.At("SoftPrice"),
                    SoftPriceCode);

                ValidationRules.Positive(
                    promotion.HardPrice,
                    promotionContext.At("HardPrice"),
                    HardPriceCode);

                IdentifierValidator.Validate(
                    promotion.RewardBundleId,
                    promotionContext.At("RewardBundleId"));
            }
        }
    }
}