using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace LL.Validation
{
    internal static class ValidationChecks
    {
        internal static bool IsNull<T>(T value) =>
            value is Object unityObject
                ? unityObject == null
                : value is null;

        internal static bool IsNotNull<T>(T value) =>
            IsNull(value) is false;

        internal static bool IsEmpty(string value) =>
            string.IsNullOrWhiteSpace(value);

        internal static bool IsNotEmpty(string value) =>
            IsEmpty(value) is false;

        internal static bool IsTrimmed(string value) =>
            value == value?.Trim();

        internal static bool IsNotTrimmed(string value) =>
            IsTrimmed(value) is false;

        internal static bool IsUppercase(string value) =>
            value == value?.ToUpperInvariant();

        internal static bool IsNotUppercase(string value) =>
            IsUppercase(value) is false;

        internal static bool IsLowercase(string value) =>
            value == value?.ToLowerInvariant();

        internal static bool IsNotLowercase(string value) =>
            IsLowercase(value) is false;

        internal static bool IsEmpty<T>(IReadOnlyCollection<T> values) =>
            values is not { Count: > 0 };

        internal static bool IsNotEmpty<T>(IReadOnlyCollection<T> values) =>
            IsEmpty(values) is false;

        internal static bool IsPositive(int value) =>
            value > 0;

        internal static bool IsNonPositive(int value) =>
            IsPositive(value) is false;

        internal static bool IsNegative(int value) =>
            value < 0;

        internal static bool IsNonNegative(int value) =>
            IsNegative(value) is false;

        internal static bool AreEqual<T>(T value, T expectedValue, IEqualityComparer<T> comparer = null) =>
            (comparer ?? EqualityComparer<T>.Default).Equals(value, expectedValue);

        internal static bool AreNotEqual<T>(T value, T forbiddenValue, IEqualityComparer<T> comparer = null) =>
            AreEqual(value, forbiddenValue, comparer) is false;

        internal static bool IsLessThanOrEqual(int value, int maximumValue) =>
            value <= maximumValue;

        internal static bool IsGreaterThan(int value, int maximumValue) =>
            IsLessThanOrEqual(value, maximumValue) is false;

        internal static bool IsDefined<TEnum>(TEnum value) where TEnum : struct, Enum =>
            Enum.IsDefined(typeof(TEnum), value);

        internal static bool IsUndefined<TEnum>(TEnum value) where TEnum : struct, Enum =>
            IsDefined(value) is false;

        internal static bool Contains<T>(T value, ISet<T> availableValues) =>
            availableValues is not null && availableValues.Contains(value);

        internal static bool DoesNotContain<T>(T value, ISet<T> availableValues) =>
            Contains(value, availableValues) is false;

        internal static bool TryAddUnique<T>(T value, ISet<T> usedValues) =>
            usedValues is not null && usedValues.Add(value);
    }
}