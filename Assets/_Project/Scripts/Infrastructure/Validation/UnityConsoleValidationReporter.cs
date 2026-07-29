using System;
using LL.Validation.Reporting;
using LL.Validation;
using UnityEngine;
using VContainer;

namespace LL.Infrastructure.Validation
{
    internal sealed class UnityConsoleValidationReporter : IValidationReporter
    {
        private readonly IValidationIssueFormatter _formatter;

        internal UnityConsoleValidationReporter()
            : this(new ValidationIssueFormatter())
        {
        }

        [Inject]
        internal UnityConsoleValidationReporter(IValidationIssueFormatter formatter)
        {
            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
        }

        public void Report(ValidationResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            foreach (var issue in result.Issues)
            {
                var message = _formatter.Format(issue);

                switch (issue.Severity)
                {
                    case ValidationSeverity.Info:
                        Debug.Log(message);
                        break;

                    case ValidationSeverity.Warning:
                        Debug.LogWarning(message);
                        break;

                    case ValidationSeverity.Error:
                        Debug.LogError(message);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(
                            nameof(issue.Severity),
                            issue.Severity,
                            "Validation severity is not supported.");
                }
            }
        }
    }
}