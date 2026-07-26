using System;
using LL.Identifiers;

namespace LL.Game.ExperienceCards
{
    internal readonly struct ExperienceCardId : IIdentifier, IEquatable<ExperienceCardId>
    {
        private readonly StringId _value;

        internal string Value => _value.Value;
        public bool IsEmpty => _value.IsEmpty;

        internal ExperienceCardId(string value)
        {
            _value = new StringId(value);
        }

        public bool Equals(ExperienceCardId other) => _value.Equals(other._value);

        public override bool Equals(object obj) => obj is ExperienceCardId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => _value.ToString();

        public static bool operator ==(ExperienceCardId left, ExperienceCardId right) => left.Equals(right);
        public static bool operator !=(ExperienceCardId left, ExperienceCardId right) => left.Equals(right) is false;
    }
}