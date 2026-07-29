using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;

namespace LL.Validation
{
    internal sealed class ValidationResult
    {
        private readonly List<ValidationIssue> _issues = new();
        private readonly ReadOnlyCollection<ValidationIssue> _readOnlyIssues;

        internal bool IsValid => _issues.All(issue => issue.Severity != ValidationSeverity.Error);
        internal IReadOnlyList<ValidationIssue> Issues => _readOnlyIssues;

        internal ValidationResult()
        {
            _readOnlyIssues = _issues.AsReadOnly();
        }

        internal void Add(ValidationIssue issue)
        {
            if (issue == null)
                throw new ArgumentNullException(nameof(issue));

            _issues.Add(issue);
        }
    }
}