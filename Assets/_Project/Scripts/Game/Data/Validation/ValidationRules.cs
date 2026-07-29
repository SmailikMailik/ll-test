using System;
using System.Collections.Generic;

namespace LL.Game.Data.Validation
{
    internal static class ValidationRules
    {
        internal static bool NotNull<T>(
            T value,
            ValidationContext context,
            string code)
        {
            EnsureArguments(context, code);

            if (value is not null)
                return true;

            return ReportError(context, code, "Value must not be null.");
        }

        internal static bool NotEmpty(
            string value,
            ValidationContext context,
            string code)
        {
            EnsureArguments(context, code);

            if (string.IsNullOrWhiteSpace(value) is false)
                return true;

            return ReportError(context, code, "Value must not be empty.");
        }

        internal static bool Trimmed(
            string value,
            ValidationContext context,
            string code)
        {
            EnsureArguments(context, code);

            if (value == value?.Trim())
                return true;

            return ReportError(context, code, "Value must not contain leading or trailing whitespace.");
        }

        internal static bool Uppercase(
            string value,
            ValidationContext context,
            string code)
        {
            EnsureArguments(context, code);

            if (value == value?.ToUpperInvariant())
                return true;

            return ReportError(context, code, "Value must be uppercase.");
        }

        internal static bool Lowercase(
            string value,
            ValidationContext context,
            string code)
        {
            EnsureArguments(context, code);

            if (value == value?.ToLowerInvariant())
                return true;

            return ReportError(context, code, "Value must be lowercase.");
        }

        internal static bool NotEmpty<T>(
            IReadOnlyCollection<T> values,
            ValidationContext context,
            string code)
        {
            EnsureArguments(context, code);

            if (values != null && values.Count > 0)
                return true;

            return ReportError(context, code, "Collection must contain at least one value.");
        }

        internal static bool Positive(
            int value,
            ValidationContext context,
            string code)
        {
            EnsureArguments(context, code);

            if (value > 0)
                return true;

            return ReportError(context, code, "Value must be greater than zero.");
        }

        internal static bool NonNegative(
            int value,
            ValidationContext context,
            string code)
        {
            EnsureArguments(context, code);

            if (value >= 0)
                return true;

            return ReportError(context, code, "Value must not be negative.");
        }

        internal static bool DefinedEnum<TEnum>(
            TEnum value,
            ValidationContext context,
            string code)
            where TEnum : struct, Enum
        {
            EnsureArguments(context, code);

            if (Enum.IsDefined(typeof(TEnum), value))
                return true;

            return ReportError(context, code, $"Value '{value}' is not supported.");
        }

        internal static bool ReferenceExists<T>(
            T value,
            ISet<T> availableValues,
            ValidationContext context,
            string code)
        {
            EnsureArguments(context, code);

            if (availableValues == null)
                throw new ArgumentNullException(nameof(availableValues));

            if (availableValues.Contains(value))
                return true;

            return ReportError(context, code, $"Referenced value '{value}' does not exist.");
        }

        internal static bool Unique<T>(
            IEnumerable<T> values,
            ValidationContext context,
            string code,
            IEqualityComparer<T> comparer = null)
        {
            EnsureArguments(context, code);

            if (values == null)
                return true;

            var usedValues = new HashSet<T>(comparer);
            var isValid = true;
            var index = 0;

            foreach (var value in values)
            {
                if (usedValues.Add(value) is false)
                {
                    context
                        .At(index)
                        .Report(
                            ValidationSeverity.Error,
                            code,
                            $"Value '{value}' must be unique.");

                    isValid = false;
                }

                index++;
            }

            return isValid;
        }

        private static bool ReportError(
            ValidationContext context,
            string code,
            string message)
        {
            context.Report(ValidationSeverity.Error, code, message);
            return false;
        }

        private static void EnsureArguments(
            ValidationContext context,
            string code)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Validation issue code must be non-empty.", nameof(code));
        }
    }
}