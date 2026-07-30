using System;
using LL.Game.Identifiers;

namespace LL.Game.Ranks
{
    internal readonly struct RankId : IIdentifier, IEquatable<RankId>
    {
        public string Value { get; }

        internal RankId(string value) => Value = value;

        public bool Equals(RankId other) => StringComparer.Ordinal.Equals(Value, other.Value);

        public override bool Equals(object obj) => obj is RankId other && Equals(other);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
        public override string ToString() => Value ?? string.Empty;
    }
}