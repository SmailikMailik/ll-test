using System;
using LL.Game.Identifiers;

namespace LL.Game.RankUp
{
    internal readonly struct RankUpRequirementId : IIdentifier, IEquatable<RankUpRequirementId>
    {
        public string Value { get; }

        internal RankUpRequirementId(string value) => Value = value;

        public bool Equals(RankUpRequirementId other) => StringComparer.Ordinal.Equals(Value, other.Value);

        public override bool Equals(object obj) => obj is RankUpRequirementId other && Equals(other);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
        public override string ToString() => Value ?? string.Empty;
    }
}