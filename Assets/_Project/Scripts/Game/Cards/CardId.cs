using System;
using LL.Identifiers;

namespace LL.Game.Cards
{
    internal readonly struct CardId : IIdentifier, IEquatable<CardId>
    {
        private readonly StringId _value;

        internal string Value => _value.Value;
        public bool IsEmpty => _value.IsEmpty;

        internal CardId(string value)
        {
            _value = new StringId(value);
        }

        public bool Equals(CardId other) => _value.Equals(other._value);

        public override bool Equals(object obj) => obj is CardId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => _value.ToString();

        public static bool operator ==(CardId left, CardId right) => left.Equals(right);
        public static bool operator !=(CardId left, CardId right) => left.Equals(right) is false;
    }
}