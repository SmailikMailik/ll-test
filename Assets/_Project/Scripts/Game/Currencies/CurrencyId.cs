using System;
using LL.Identifiers;

namespace LL.Game.Currencies
{
    internal readonly struct CurrencyId : IIdentifier, IEquatable<CurrencyId>
    {
        internal string Value { get; }
        public bool IsEmpty => string.IsNullOrEmpty(Value);

        internal CurrencyId(string value) => Value = IdentifierNormalizer.Normalize(value);

        public bool Equals(CurrencyId other) => StringComparer.Ordinal.Equals(Value, other.Value);

        public override bool Equals(object obj) => obj is CurrencyId other && Equals(other);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
        public override string ToString() => Value ?? string.Empty;
    }
}