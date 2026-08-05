using System.Collections.Generic;
using LL.Game.Identifiers;
using LL.Game.Payments.Configuration;
using LL.Validation;

namespace LL.Game.RankUp.Configuration
{
    internal sealed class RankUpCatalogConfigValidator : IDataValidator<RankUpEntry[]>
    {
        private const string EntriesCode = "rank-up.entries.required";
        private const string OptionsCode = "rank-up.options.not-empty";
        private const string RequirementsCode = "rank-up.option.requirements.required";
        private const string RequirementCode = "rank-up.requirement.required";
        private const string RequirementTypeCode = "rank-up.requirement.type.supported";
        private const string RequiredCountCode = "rank-up.quest.count.positive";
        private const string DurationCode = "rank-up.quest.duration.positive";

        private static readonly IDataValidator<PaymentEntry> _paymentValidator = new PaymentEntryValidator();

        public void Validate(
            RankUpEntry[] rankUps,
            ValidationContext context)
        {
            if (ValidationRules.NotNull(rankUps, context, EntriesCode) is false)
                return;

            for (var index = 0; index < rankUps.Length; index++)
            {
                var rankUp = rankUps[index];

                if (rankUp is null)
                    continue;

                ValidateRankUp(rankUp, context.At(index));
            }
        }

        private static void ValidateRankUp(
            RankUpEntry rankUp,
            ValidationContext context)
        {
            IdentifierValidator.Validate(rankUp.HeroId, context.At(nameof(RankUpEntry.HeroId)));
            IdentifierValidator.Validate(rankUp.RankId, context.At(nameof(RankUpEntry.RankId)));
            IdentifierValidator.Validate(rankUp.RewardId, context.At(nameof(RankUpEntry.RewardId)));

            if (ValidationRules.NotEmpty(rankUp.Options, context.At(nameof(RankUpEntry.Options)), OptionsCode) is false)
                return;

            IdentifierCollectionValidator.Validate(
                rankUp.Options,
                option => option.OptionId,
                context.At(nameof(RankUpEntry.Options)));

            for (var index = 0; index < rankUp.Options.Count; index++)
            {
                var option = rankUp.Options[index];

                if (option is not null)
                    ValidateOption(option, context.At(nameof(RankUpEntry.Options)).At(index));
            }
        }

        private static void ValidateOption(
            RankUpOptionEntry option,
            ValidationContext context)
        {
            if (ValidationRules.NotNull(
                    option.Requirements,
                    context.At(nameof(option.Requirements)),
                    RequirementsCode) is false)
                return;

            var requirementIds = new HashSet<RankUpRequirementId>();

            for (var index = 0; index < option.Requirements.Count; index++)
            {
                var requirement = option.Requirements[index];

                if (requirement is null)
                    continue;

                var requirementContext = context.At(nameof(option.Requirements)).At(index);
                ValidateRequirementId(requirement.RequirementId, requirementIds, requirementContext);
                ValidateRequirement(requirement, requirementContext);
            }
        }

        private static void ValidateRequirement(
            RankUpRequirementEntry requirement,
            ValidationContext context)
        {
            switch (requirement)
            {
                case QuestRankUpRequirementEntry quest:
                    IdentifierValidator.Validate(quest.QuestId, context.At(nameof(quest.QuestId)));
                    ValidationRules.Positive(
                        quest.RequiredCount,
                        context.At(nameof(quest.RequiredCount)),
                        RequiredCountCode);
                    ValidationRules.Positive(
                        quest.DurationMinutes,
                        context.At(nameof(quest.DurationMinutes)),
                        DurationCode);
                    break;
                case PaymentRankUpRequirementEntry payment:
                    _paymentValidator.Validate(payment.Payment, context.At(nameof(payment.Payment)));
                    break;
                default:
                    context.Report(
                        ValidationSeverity.Error,
                        RequirementTypeCode,
                        $"Unsupported rank-up requirement type '{requirement.GetType().Name}'.");
                    break;
            }
        }

        private static void ValidateRequirementId(
            RankUpRequirementId requirementId,
            ISet<RankUpRequirementId> usedIds,
            ValidationContext context)
        {
            IdentifierValidator.Validate(requirementId, context.At("RequirementId"));
            ValidationRules.TryAddUnique(requirementId, usedIds, context.At("RequirementId"), RequirementCode);
        }
    }
}