using System;

namespace LL.Identifiers
{
    internal readonly struct StringId : IEquatable<StringId>
    {
        internal string Value { get; }
        internal bool IsEmpty => string.IsNullOrEmpty(Value);

        internal StringId(string value)
        {
            Value = value?.Trim().ToLowerInvariant() ?? string.Empty;
        }

        public bool Equals(StringId other)
        {
            return string.Equals(
                Value ?? string.Empty,
                other.Value ?? string.Empty,
                StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is StringId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
        }

        public override string ToString() => Value ?? string.Empty;

        public static bool operator ==(StringId left, StringId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(StringId left, StringId right)
        {
            return left.Equals(right) is false;
        }
    }
}