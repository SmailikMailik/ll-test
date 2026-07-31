using System;
using LL.Game.Identifiers;

namespace LL.Game.Countries
{
    internal readonly struct CountryId : IIdentifier, IEquatable<CountryId>
    {
        public string Value { get; }

        internal CountryId(string value) => Value = value;

        public bool Equals(CountryId other) => StringComparer.Ordinal.Equals(Value, other.Value);

        public override bool Equals(object obj) => obj is CountryId other && Equals(other);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
        public override string ToString() => Value ?? string.Empty;
    }
}