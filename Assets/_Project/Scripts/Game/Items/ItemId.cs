using System;
using LL.Identifiers;

namespace LL.Game.Items
{
    internal readonly struct ItemId : IIdentifier, IEquatable<ItemId>
    {
        internal string Value { get; }
        public bool IsEmpty => string.IsNullOrEmpty(Value);

        internal ItemId(string value) => Value = IdentifierNormalizer.Normalize(value);

        public bool Equals(ItemId other) => StringComparer.Ordinal.Equals(Value, other.Value);

        public override bool Equals(object obj) => obj is ItemId other && Equals(other);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
        public override string ToString() => Value ?? string.Empty;
    }
}