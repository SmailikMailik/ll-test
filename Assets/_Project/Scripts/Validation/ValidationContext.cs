using System;

namespace LL.Validation
{
    internal sealed class ValidationContext
    {
        private readonly ValidationResult _result;

        internal string Path { get; }

        internal ValidationContext(ValidationResult result)
            : this(result, string.Empty)
        {
        }

        private ValidationContext(ValidationResult result, string path)
        {
            _result = result ?? throw new ArgumentNullException(nameof(result));
            Path = path;
        }

        internal ValidationContext At(string segment)
        {
            if (string.IsNullOrWhiteSpace(segment))
                throw new ArgumentException("Validation path segment must be non-empty.", nameof(segment));

            var path = string.IsNullOrEmpty(Path)
                ? segment
                : $"{Path}.{segment}";

            return new ValidationContext(_result, path);
        }

        internal ValidationContext At(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(index),
                    index,
                    "Validation path index cannot be negative.");

            return new ValidationContext(_result, $"{Path}[{index}]");
        }

        internal void Report(
            ValidationSeverity severity,
            string code,
            string message)
        {
            _result.Add(new ValidationIssue(severity, code, Path, message));
        }
    }
}