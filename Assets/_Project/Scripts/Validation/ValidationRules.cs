using System;
using System.Collections.Generic;

namespace LL.Validation
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

            ReportError(context, code, "Value must not be null.");
            return false;
        }

        internal static bool NotEmpty(
            string value,
            ValidationContext context,
            string code)
        {
            EnsureArguments(context, code);

            if (string.IsNullOrWhiteSpace(value) is false)
                return true;

            ReportError(context, code, "Value must not be empty.");
            return false;
        }

        internal static bool Trimmed(
            string value,
            ValidationContext context,
            string code)
        {
            EnsureArguments(context, code);

            if (value == value?.Trim())
                return true;

            ReportError(context, code, "Value must not contain leading or trailing whitespace.");
            return false;
        }

        internal static bool Uppercase(
            string value,
            ValidationContext context,
            string code)
        {
            EnsureArguments(context, code);

            if (value == value?.ToUpperInvariant())
                return true;

            ReportError(context, code, "Value must be uppercase.");
            return false;
        }

        internal static bool Lowercase(
            string value,
            ValidationContext context,
            string code)
        {
            EnsureArguments(context, code);

            if (value == value?.ToLowerInvariant())
                return true;

            ReportError(context, code, "Value must be lowercase.");
            return false;
        }

        internal static bool NotEmpty<T>(
            IReadOnlyCollection<T> values,
            ValidationContext context,
            string code)
        {
            EnsureArguments(context, code);

            if (values != null && values.Count > 0)
                return true;

            ReportError(context, code, "Collection must contain at least one value.");
            return false;
        }

        internal static bool Positive(
            int value,
            ValidationContext context,
            string code)
        {
            EnsureArguments(context, code);

            if (value > 0)
                return true;

            ReportError(context, code, "Value must be greater than zero.");
            return false;
        }

        internal static bool NonNegative(
            int value,
            ValidationContext context,
            string code)
        {
            EnsureArguments(context, code);

            if (value >= 0)
                return true;

            ReportError(context, code, "Value must not be negative.");
            return false;
        }

        internal static bool Equal<T>(
            T value,
            T expectedValue,
            ValidationContext context,
            string code,
            IEqualityComparer<T> comparer = null)
        {
            EnsureArguments(context, code);

            comparer ??= EqualityComparer<T>.Default;

            if (comparer.Equals(value, expectedValue))
                return true;

            ReportError(context, code, $"Value '{value}' must equal '{expectedValue}'.");
            return false;
        }

        internal static bool NotEqual<T>(
            T value,
            T forbiddenValue,
            ValidationContext context,
            string code,
            IEqualityComparer<T> comparer = null)
        {
            EnsureArguments(context, code);

            comparer ??= EqualityComparer<T>.Default;

            if (comparer.Equals(value, forbiddenValue) is false)
                return true;

            ReportError(context, code, $"Value '{value}' must not equal '{forbiddenValue}'.");
            return false;
        }

        internal static bool LessThanOrEqual(
            int value,
            int maximumValue,
            ValidationContext context,
            string code)
        {
            EnsureArguments(context, code);

            if (value <= maximumValue)
                return true;

            ReportError(context, code, $"Value '{value}' must not exceed '{maximumValue}'.");
            return false;
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

            ReportError(context, code, $"Value '{value}' is not supported.");
            return false;
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

            ReportError(context, code, $"Referenced value '{value}' does not exist.");
            return false;
        }

        internal static bool Unique<T>(
            T value,
            ISet<T> usedValues,
            ValidationContext context,
            string code)
        {
            EnsureArguments(context, code);

            if (usedValues == null)
                throw new ArgumentNullException(nameof(usedValues));

            if (usedValues.Add(value))
                return true;

            ReportError(context, code, $"Value '{value}' must be unique.");
            return false;
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
                if (Unique(value, usedValues, context.At(index), code) is false)
                    isValid = false;

                index++;
            }

            return isValid;
        }

        private static void ReportError(
            ValidationContext context,
            string code,
            string message)
        {
            context.Report(ValidationSeverity.Error, code, message);
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