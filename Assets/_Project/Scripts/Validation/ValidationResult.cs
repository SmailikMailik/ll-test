using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LL.Validation
{
    internal sealed class ValidationResult
    {
        private readonly List<ValidationIssue> _issues = new();
        private readonly ReadOnlyCollection<ValidationIssue> _readOnlyIssues;

        internal bool IsValid => ErrorCount == 0;
        internal IReadOnlyList<ValidationIssue> Issues => _readOnlyIssues;

        internal int ErrorCount { get; private set; }
        internal int WarningCount { get; private set; }
        internal int InfoCount { get; private set; }

        internal ValidationResult()
        {
            _readOnlyIssues = _issues.AsReadOnly();
        }

        internal void Add(ValidationIssue issue)
        {
            if (issue == null)
                throw new ArgumentNullException(nameof(issue));

            _issues.Add(issue);

            switch (issue.Severity)
            {
                case ValidationSeverity.Error:
                    ErrorCount++;
                    break;

                case ValidationSeverity.Warning:
                    WarningCount++;
                    break;

                case ValidationSeverity.Info:
                    InfoCount++;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(issue),
                        issue.Severity,
                        "Validation severity is not supported.");
            }
        }
    }
}