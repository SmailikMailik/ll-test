using LL.Game.Data.Validation;

namespace LL.Game.Identifiers
{
    internal static class IdentifierValidator
    {
        private const string EmptyCode = "identifier.empty";
        private const string WhitespaceCode = "identifier.trimmed";
        private const string CaseCode = "identifier.lowercase";

        internal static ValidationResult Validate<TId>(TId id)
            where TId : struct, IIdentifier
        {
            var result = new ValidationResult();
            Validate(id, new ValidationContext(result));
            return result;
        }

        internal static void Validate<TId>(
            TId id,
            ValidationContext context)
            where TId : struct, IIdentifier
        {
            if (ValidationRules.NotEmpty(
                id.Value,
                context,
                EmptyCode) is false)
            {
                return;
            }

            ValidationRules.Trimmed(
                id.Value,
                context,
                WhitespaceCode);

            ValidationRules.Lowercase(
                id.Value,
                context,
                CaseCode);
        }

        internal static bool IsValid<TId>(TId id)
            where TId : struct, IIdentifier
        {
            return Validate(id).IsValid;
        }

        internal static void EnsureValid<TId>(
            TId id,
            string parameterName)
            where TId : struct, IIdentifier
        {
            ValidationResultGuard.EnsureValid(
                Validate(id),
                parameterName);
        }
    }
}