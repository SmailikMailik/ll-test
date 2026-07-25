using System;

namespace LL.Game.ExperienceCards
{
    internal readonly struct ExperienceCardId : IEquatable<ExperienceCardId>
    {
        internal string Value { get; }
        internal bool IsEmpty => string.IsNullOrEmpty(Value);

        internal ExperienceCardId(string value)
        {
            Value = value?.Trim().ToLowerInvariant() ?? string.Empty;
        }

        public bool Equals(ExperienceCardId other)
        {
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is ExperienceCardId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
        }

        public override string ToString() => Value ?? string.Empty;

        public static bool operator ==(ExperienceCardId left, ExperienceCardId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ExperienceCardId left, ExperienceCardId right)
        {
            return left.Equals(right) is false;
        }
    }
}