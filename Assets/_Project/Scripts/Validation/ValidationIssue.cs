using System;

namespace LL.Validation
{
    internal sealed class ValidationIssue
    {
        internal ValidationSeverity Severity { get; }
        internal string Code { get; }
        internal string Path { get; }
        internal string Message { get; }

        internal ValidationIssue(
            ValidationSeverity severity,
            string code,
            string path,
            string message)
        {
            if (ValidationChecks.IsUndefined(severity))
                throw new ArgumentOutOfRangeException(
                    nameof(severity),
                    severity,
                    "Validation severity is not supported.");

            if (ValidationChecks.IsEmpty(code))
                throw new ArgumentException("Validation issue code must be non-empty.", nameof(code));

            if (ValidationChecks.IsEmpty(message))
                throw new ArgumentException("Validation issue message must be non-empty.", nameof(message));

            Severity = severity;
            Code = code;
            Path = path ?? string.Empty;
            Message = message;
        }
    }
}