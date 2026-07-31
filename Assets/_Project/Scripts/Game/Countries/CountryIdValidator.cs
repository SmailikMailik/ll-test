using LL.Game.Identifiers;
using LL.Validation;

namespace LL.Game.Countries
{
    internal static class CountryIdValidator
    {
        private const string FormatCode = "country-id.iso-alpha-2";

        internal static void Validate(CountryId id, ValidationContext context)
        {
            IdentifierValidator.Validate(id, context);

            if (IsIsoAlpha2(id.Value))
                return;

            context.Report(
                ValidationSeverity.Error,
                FormatCode,
                "Country ID must be a lowercase ISO 3166-1 alpha-2 code.");
        }

        internal static bool IsValid(CountryId id)
        {
            var result = new ValidationResult();
            Validate(id, new ValidationContext(result));
            return result.IsValid;
        }

        internal static void EnsureValid(CountryId id, string parameterName)
        {
            ValidationRunner.EnsureValid(
                context => Validate(id, context),
                parameterName);
        }

        private static bool IsIsoAlpha2(string value)
        {
            return value is { Length: 2 } &&
                   value[0] is >= 'a' and <= 'z' &&
                   value[1] is >= 'a' and <= 'z';
        }
    }
}