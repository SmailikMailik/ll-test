using System;

namespace LL.Game.Promotions
{
    internal sealed class RankPromotionRequirement
    {
        internal PromotionRequirementId Id { get; }
        internal string TitleLocalizationKey { get; }
        internal string DescriptionLocalizationKey { get; }
        internal string TargetLocalizationKey { get; }
        internal int RequiredAmount { get; }

        internal RankPromotionRequirement(
            PromotionRequirementId id,
            string titleLocalizationKey,
            string descriptionLocalizationKey,
            string targetLocalizationKey,
            int requiredAmount)
        {
            if (id.IsEmpty)
                throw new ArgumentException("Promotion requirement ID must be non-empty.", nameof(id));

            if (string.IsNullOrWhiteSpace(titleLocalizationKey))
                throw new ArgumentException("Promotion title localization key must be non-empty.");

            if (string.IsNullOrWhiteSpace(descriptionLocalizationKey))
                throw new ArgumentException("Promotion description localization key must be non-empty.");

            if (string.IsNullOrWhiteSpace(targetLocalizationKey))
                throw new ArgumentException("Promotion target localization key must be non-empty.");

            if (requiredAmount <= 0)
                throw new ArgumentOutOfRangeException(nameof(requiredAmount));

            Id = id;
            TitleLocalizationKey = titleLocalizationKey;
            DescriptionLocalizationKey = descriptionLocalizationKey;
            TargetLocalizationKey = targetLocalizationKey;
            RequiredAmount = requiredAmount;
        }
    }
}