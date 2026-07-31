using System;
using System.Collections.Generic;
using LL.Validation.Reporting;

namespace LL.Validation
{
    internal static class ValidationResultGuard
    {
        private static readonly IValidationIssueFormatter _formatter = new ValidationIssueFormatter();

        internal static void EnsureValid(
            ValidationResult result,
            string parameterName = null)
        {
            if (result is null)
                throw new ArgumentNullException(nameof(result));

            if (result.IsValid)
                return;

            throw new ArgumentException(
                FormatErrors(result),
                parameterName);
        }

        private static string FormatErrors(ValidationResult result)
        {
            var messages = new List<string>(result.ErrorCount);

            foreach (var issue in result.Issues)
            {
                if (issue.Severity == ValidationSeverity.Error)
                    messages.Add(_formatter.Format(issue));
            }

            if (messages.Count == 0)
                throw new InvalidOperationException("Invalid validation result does not contain an error.");

            return string.Join(Environment.NewLine, messages);
        }
    }
}