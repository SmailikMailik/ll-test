using System;
using LL.Game.Identifiers;

namespace LL.Game.Rewards
{
    internal readonly struct RewardId : IIdentifier, IEquatable<RewardId>
    {
        public string Value { get; }

        internal RewardId(string value) => Value = value;

        public bool Equals(RewardId other) => StringComparer.Ordinal.Equals(Value, other.Value);

        public override bool Equals(object obj) => obj is RewardId other && Equals(other);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
        public override string ToString() => Value ?? string.Empty;
    }
}