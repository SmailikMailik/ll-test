using System;
using System.Collections.Generic;
using LL.Game.Identifiers;
using LL.Validation;

namespace LL.User.Configuration
{
    internal sealed class UserDefaultsConfigValidator : IDataValidator<UserDefaultsConfig>
    {
        private const string ConfigCode = "user-defaults.config.required";
        private const string IdentityCode = "user-defaults.identity.required";
        private const string UserIdCode = "user-defaults.identity.user-id.not-empty";
        private const string UserIdWhitespaceCode = "user-defaults.identity.user-id.trimmed";
        private const string RegionCodeCode = "user-defaults.identity.region-code.not-empty";
        private const string RegionWhitespaceCode = "user-defaults.identity.region-code.trimmed";
        private const string RegionCaseCode = "user-defaults.identity.region-code.uppercase";
        private const string ProgressCode = "user-defaults.progress.required";
        private const string ExperienceCode = "user-defaults.progress.experience.non-negative";

        private readonly IDataValidator<IReadOnlyList<UserItemDefaultEntry>> _itemsValidator;

        internal UserDefaultsConfigValidator(IDataValidator<IReadOnlyList<UserItemDefaultEntry>> itemsValidator)
        {
            _itemsValidator = itemsValidator ?? throw new ArgumentNullException(nameof(itemsValidator));
        }

        public void Validate(
            UserDefaultsConfig defaults,
            ValidationContext context)
        {
            if (ValidationRules.NotNull(defaults, context, ConfigCode) is false)
                return;

            ValidateIdentity(defaults.Identity, context.At(nameof(UserDefaultsConfig.Identity)));
            ValidateProgress(defaults.Progress, context.At(nameof(UserDefaultsConfig.Progress)));
            _itemsValidator.Validate(defaults.Items, context.At(nameof(UserDefaultsConfig.Items)));
        }

        private static void ValidateIdentity(
            UserIdentityDefaults identity,
            ValidationContext context)
        {
            if (ValidationRules.NotNull(identity, context, IdentityCode) is false)
                return;

            ValidationRules.NotEmpty(
                identity.UserId,
                context.At(nameof(UserIdentityDefaults.UserId)),
                UserIdCode);

            ValidationRules.Trimmed(
                identity.UserId,
                context.At(nameof(UserIdentityDefaults.UserId)),
                UserIdWhitespaceCode);

            ValidationRules.NotEmpty(
                identity.RegionCode,
                context.At(nameof(UserIdentityDefaults.RegionCode)),
                RegionCodeCode);

            ValidationRules.Trimmed(
                identity.RegionCode,
                context.At(nameof(UserIdentityDefaults.RegionCode)),
                RegionWhitespaceCode);

            ValidationRules.Uppercase(
                identity.RegionCode,
                context.At(nameof(UserIdentityDefaults.RegionCode)),
                RegionCaseCode);
        }

        private static void ValidateProgress(
            UserProgressDefaults progress,
            ValidationContext context)
        {
            if (ValidationRules.NotNull(progress, context, ProgressCode) is false)
                return;

            IdentifierValidator.Validate(
                progress.RankId,
                context.At(nameof(UserProgressDefaults.RankId)));

            ValidationRules.NonNegative(
                progress.Experience,
                context.At(nameof(UserProgressDefaults.Experience)),
                ExperienceCode);
        }
    }
}