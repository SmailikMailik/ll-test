using System;
using LL.Game.Identifiers;

namespace LL.Game.Heroes
{
    internal readonly struct HeroId : IIdentifier, IEquatable<HeroId>
    {
        public string Value { get; }

        internal HeroId(string value) => Value = value;

        public bool Equals(HeroId other) => StringComparer.Ordinal.Equals(Value, other.Value);

        public override bool Equals(object obj) => obj is HeroId other && Equals(other);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
        public override string ToString() => Value ?? string.Empty;
    }
}