using System;
using LL.Game.Identifiers;

namespace LL.Tests.EditMode.Game.Identifiers
{
    internal readonly struct TestIdentifier : IIdentifier, IEquatable<TestIdentifier>
    {
        public string Value { get; }

        internal TestIdentifier(string value) => Value = value;

        public bool Equals(TestIdentifier other) => StringComparer.Ordinal.Equals(Value, other.Value);
        public override bool Equals(object obj) => obj is TestIdentifier other && Equals(other);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
    }
}