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
        private const string QuestsCode = "rank-up.option.quests.required";
        private const string PaymentsCode = "rank-up.option.payments.required";
        private const string RequirementCode = "rank-up.requirement.required";
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
            if (ValidationRules.NotNull(option.Quests, context.At(nameof(option.Quests)), QuestsCode) is false ||
                ValidationRules.NotNull(option.Payments, context.At(nameof(option.Payments)), PaymentsCode) is false)
            {
                return;
            }

            var requirementIds = new HashSet<RankUpRequirementId>();

            for (var index = 0; index < option.Quests.Count; index++)
            {
                var quest = option.Quests[index];

                if (quest is null)
                    continue;

                var questContext = context.At(nameof(option.Quests)).At(index);
                ValidateRequirementId(quest.RequirementId, requirementIds, questContext);
                IdentifierValidator.Validate(quest.QuestId, questContext.At(nameof(quest.QuestId)));
                ValidationRules.Positive(
                    quest.RequiredCount,
                    questContext.At(nameof(quest.RequiredCount)),
                    RequiredCountCode);
                ValidationRules.Positive(
                    quest.DurationMinutes,
                    questContext.At(nameof(quest.DurationMinutes)),
                    DurationCode);
            }

            for (var index = 0; index < option.Payments.Count; index++)
            {
                var payment = option.Payments[index];

                if (payment is null)
                    continue;

                var paymentContext = context.At(nameof(option.Payments)).At(index);
                ValidateRequirementId(payment.RequirementId, requirementIds, paymentContext);
                _paymentValidator.Validate(payment.Payment, paymentContext.At(nameof(payment.Payment)));
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