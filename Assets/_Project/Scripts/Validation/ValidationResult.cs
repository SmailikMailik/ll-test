using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

namespace LL.Validation
{
    internal sealed class ValidationResult
    {
        private readonly List<ValidationIssue> _issues = new();
        private readonly ReadOnlyCollection<ValidationIssue> _readOnlyIssues;
        private int _errorCount;
        private int _infoCount;
        private int _warningCount;

        internal bool IsValid => _errorCount == 0;
        internal IReadOnlyList<ValidationIssue> Issues => _readOnlyIssues;
        internal int ErrorCount => _errorCount;
        internal int InfoCount => _infoCount;
        internal int WarningCount => _warningCount;

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
                case ValidationSeverity.Info:
                    _infoCount++;
                    break;

                case ValidationSeverity.Warning:
                    _warningCount++;
                    break;

                case ValidationSeverity.Error:
                    _errorCount++;
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