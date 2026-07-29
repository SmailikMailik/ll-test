using System;

namespace LL.Validation.Reporting
{
    internal sealed class ValidationIssueFormatter : IValidationIssueFormatter
    {
        public string Format(ValidationIssue issue)
        {
            if (issue == null)
                throw new ArgumentNullException(nameof(issue));

            var location = string.IsNullOrEmpty(issue.Path)
                ? string.Empty
                : $" at '{issue.Path}'";

            return $"[{issue.Severity}] {issue.Code}{location}: {issue.Message}";
        }
    }
}