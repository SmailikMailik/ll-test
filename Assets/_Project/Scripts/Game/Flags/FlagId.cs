using System;
using LL.Game.Identifiers;

namespace LL.Game.Flags
{
    internal readonly struct FlagId : IIdentifier, IEquatable<FlagId>
    {
        public string Value { get; }

        internal FlagId(string value) => Value = value;

        public bool Equals(FlagId other) => StringComparer.Ordinal.Equals(Value, other.Value);

        public override bool Equals(object obj) => obj is FlagId other && Equals(other);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
        public override string ToString() => Value ?? string.Empty;
    }
}