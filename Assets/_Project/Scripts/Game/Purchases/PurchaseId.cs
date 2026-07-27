using System;
using LL.Identifiers;

namespace LL.Game.Purchases
{
    internal readonly struct PurchaseId : IIdentifier, IEquatable<PurchaseId>
    {
        internal string Value { get; }
        public bool IsEmpty => string.IsNullOrEmpty(Value);

        internal PurchaseId(string value) => Value = IdentifierNormalizer.Normalize(value);

        public bool Equals(PurchaseId other) => StringComparer.Ordinal.Equals(Value, other.Value);

        public override bool Equals(object obj) => obj is PurchaseId other && Equals(other);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
        public override string ToString() => Value ?? string.Empty;
    }
}