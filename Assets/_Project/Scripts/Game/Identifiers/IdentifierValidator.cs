using LL.Validation;

namespace LL.Game.Identifiers
{
    internal static class IdentifierValidator
    {
        private const string EmptyCode = "identifier.empty";
        private const string WhitespaceCode = "identifier.trimmed";
        private const string CaseCode = "identifier.lowercase";

        internal static ValidationResult Validate<TId>(TId id) where TId : struct, IIdentifier =>
            ValidationRunner.Run(context => Validate(id, context));

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

        internal static bool IsValid<TId>(TId id) where TId : struct, IIdentifier =>
            ValidationRunner.IsValid(context => Validate(id, context));

        internal static void EnsureValid<TId>(
            TId id,
            string parameterName)
            where TId : struct, IIdentifier =>
            ValidationRunner.EnsureValid(context => Validate(id, context), parameterName);
    }
}