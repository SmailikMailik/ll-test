using System;
using LL.Identifiers;

namespace LL.Game.Cards
{
    internal readonly struct CardId : IIdentifier, IEquatable<CardId>
    {
        internal string Value { get; }
        public bool IsEmpty => string.IsNullOrEmpty(Value);

        internal CardId(string value) => Value = IdentifierNormalizer.Normalize(value);

        public bool Equals(CardId other) => StringComparer.Ordinal.Equals(Value, other.Value);

        public override bool Equals(object obj) => obj is CardId other && Equals(other);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
        public override string ToString() => Value ?? string.Empty;
    }
}