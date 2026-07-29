using System;
using LL.Game.Identifiers;

namespace LL.Game.Rewards
{
    internal readonly struct RewardBundleId : IIdentifier, IEquatable<RewardBundleId>
    {
        public string Value { get; }

        internal RewardBundleId(string value) => Value = value;

        public bool Equals(RewardBundleId other) => StringComparer.Ordinal.Equals(Value, other.Value);

        public override bool Equals(object obj) => obj is RewardBundleId other && Equals(other);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
        public override string ToString() => Value ?? string.Empty;
    }
}