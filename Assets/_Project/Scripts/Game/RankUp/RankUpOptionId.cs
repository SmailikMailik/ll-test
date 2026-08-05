using System;
using LL.Game.Identifiers;

namespace LL.Game.RankUp
{
    internal readonly struct RankUpOptionId : IIdentifier, IEquatable<RankUpOptionId>
    {
        public string Value { get; }

        internal RankUpOptionId(string value) => Value = value;

        public bool Equals(RankUpOptionId other) => StringComparer.Ordinal.Equals(Value, other.Value);

        public override bool Equals(object obj) => obj is RankUpOptionId other && Equals(other);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
        public override string ToString() => Value ?? string.Empty;
    }
}