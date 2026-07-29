using System;

namespace LL.Game.Data.Validation
{
    internal static class ValidationResultGuard
    {
        private static readonly IValidationIssueFormatter _formatter = new ValidationIssueFormatter();

        internal static void EnsureValid(ValidationResult result)
        {
            EnsureValid(result, null);
        }

        internal static void EnsureValid(
            ValidationResult result,
            string parameterName)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            if (result.IsValid)
                return;

            foreach (var issue in result.Issues)
            {
                if (issue.Severity != ValidationSeverity.Error)
                    continue;

                throw new ArgumentException(
                    _formatter.Format(issue),
                    parameterName);
            }

            throw new InvalidOperationException("Invalid validation result does not contain an error.");
        }
    }
}